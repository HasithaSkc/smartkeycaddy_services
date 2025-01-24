using Microsoft.Azure.Devices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Common.JsonHelper;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;
using SmartKeyCaddy.Models.Exceptions;
using SmartKeyCaddy.Models.Messages;

namespace SmartKeyCaddy.Domain.Services;

public partial class KeyAllocationService : IKeyAllocationService
{
    private readonly ILogger<KeyAllocationService> _logger;
    private readonly IIotHubServiceClient _iotHubServiceClient;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IKeyAllocationRepository _keyAllocationRepository;
    private readonly IPropertyRoomRepository _propertyRoomRepository;
    private readonly IBinRepository _binRepository;
    private readonly IKeyTransactionReposiotry _keyTransactionReposiotry;

   public KeyAllocationService(ILogger<KeyAllocationService> logger,
        IIotHubServiceClient iotHubServiceClient,
        IDeviceRepository deviceRepository,
        IKeyAllocationRepository keyRepository,
        IPropertyRoomRepository propertyRoomRepository,
        IBinRepository binRepository,
        IKeyTransactionReposiotry keyTransactionReposiotry)
    {
        _logger = logger;
        _iotHubServiceClient = iotHubServiceClient;
        _deviceRepository = deviceRepository;
        _keyAllocationRepository = keyRepository;
        _propertyRoomRepository = propertyRoomRepository;
        _binRepository = binRepository;
        _keyTransactionReposiotry = keyTransactionReposiotry;
    }

    public async Task<KeyAllocationResponse> CreateKeyAllocation(KeyAllocationRequest keyAllocationRequest)
    {
        var device = await _deviceRepository.GetDevice(keyAllocationRequest.DeviceId);

        if (device == null)
            throw new NotFoundException("Device not found");


        keyAllocationRequest.DeviceName = device.DeviceName;

        var keyAllocationResponse = new KeyAllocationResponse();
        var keysNotCreated = new List<KeyAllocation>();

        try
        {
            var keyAllocationType = await GetKeyAllocationType(device);
            var keyAllocationList = await PrepareKeyAllocationRequest(keyAllocationRequest.KeyAllocation, device, keyAllocationType);

            if (!await _iotHubServiceClient.IsDeviceOnline(device.DeviceName))
            {
                _logger.LogError($"Device is offline. Creating keys on the server");
                await InsertOrUpdateKeyAllocationList(keyAllocationList, keyAllocationType, device.PropertyId);
                return ConvertToKeyAllocationResponse(keyAllocationList, device);
            }

            await ValidateKeyAllocationList(keyAllocationList, device.PropertyId);

            // Invoke the direct method on the device
            var methodInvocation = new CloudToDeviceMethod(Constants.KeyAllocationRequestHandler) { ResponseTimeout = TimeSpan.FromSeconds(20) };

            var deviceKeyAllocationList = GetCloudToDeviceKeyAllocationRequest(keyAllocationList, device, MessageType.DirectKeyAllocation);

            // Nothing to send to the device. Return the original list.
            if (!(deviceKeyAllocationList?.KeyAllocation?.Any() ?? false))
                return ConvertToKeyAllocationResponse(keyAllocationList, device);

            methodInvocation.SetPayloadJson(JsonConvert.SerializeObject(deviceKeyAllocationList, JsonHelper.GetJsonSerializerSettings()));
            var deviceToCloudResponse = await _iotHubServiceClient.SendDirectMessageToDevice(device.DeviceName, methodInvocation);

            if (deviceToCloudResponse?.Status != DeviceResponseStatus.Success)
                throw new Exception("Key allocation failed. Invalid device status response.");

            var deviceKeyAllocationResponse = ServiceHelper.GetDeviceKeyAllocationResponse(deviceToCloudResponse.GetPayloadAsJson());

            if (!(deviceKeyAllocationResponse?.KeyAllocation?.Any() ?? false))
                throw new Exception("Key allocation failed");

            foreach (var keyAllocation in keyAllocationList)
            {
                var allocatedKey = deviceKeyAllocationResponse.KeyAllocation.SingleOrDefault(keyResponse => string.Equals(keyAllocation.KeyName, keyResponse.KeyName, StringComparison.OrdinalIgnoreCase));
                keyAllocation.IsSuccessful = allocatedKey?.IsSuccessful ?? false;
                keyAllocation.Status = allocatedKey?.Status ?? string.Empty;
                keyAllocation.IsMessageSent = keyAllocation.IsSuccessful;
                if (!(allocatedKey?.IsSuccessful ?? false))
                {
                    keysNotCreated.Add(keyAllocation);
                    _logger.LogError($"Unable to create the requested key {keyAllocation.KeyName} on device. Status: {allocatedKey?.Status}");
                };

                await InsertOrUpdateKeyAllocation(keyAllocation, device.PropertyId);
            }

            return ConvertToKeyAllocationResponse(keyAllocationList, device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Key allocation failed");
            throw;
        }
    }

    public async Task<bool> DeleteKey(Guid deviceId, Guid keyId)
    {
        var key = await _keyAllocationRepository.GetKeyAllocation(deviceId, keyId);

        if (key == null)
            throw new Exception("Key not found");

        var device = await _deviceRepository.GetDevice(deviceId);

        var methodInvocation = new CloudToDeviceMethod(Constants.KeyDeleteRequestHandler) { ResponseTimeout = TimeSpan.FromSeconds(30) };
        methodInvocation.SetPayloadJson(JsonConvert.SerializeObject(new DeleteKeyRequest { KeyId = keyId, DeviceId = deviceId }));

        try
        {
            // Invoke the direct method on the device
            var response = await _iotHubServiceClient.SendDirectMessageToDevice(device.DeviceName, methodInvocation);

            if (response == null || response.Status != DeviceResponseStatus.Success)
                throw new Exception("Failed to delete the key");

            await _keyAllocationRepository.InsertkeyAllocation(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteKey");
            throw;
        }

        return true;
    }

    public Task<bool> DeleteKey(Guid keyId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<KeyAllocation>> GetKeyAllocations(Guid deviceId)
    {
        return await _keyAllocationRepository.GetKeyAllocations(deviceId);
    }

    public async Task ProcessIndirectKeyAllocationMessages()
    {
        var unsentKeyAllocations = await _keyAllocationRepository.GetUnsentKeyAllocations();
        var distinctDeviceIds = unsentKeyAllocations.Select(key=>key.DeviceId).Distinct();

        foreach (var deviceId in distinctDeviceIds)
        {
            var device = await _deviceRepository.GetDevice(deviceId);
            var deviceKeyAllocations = unsentKeyAllocations.Where(key => key.DeviceId == deviceId).ToList();
            var deviceKeyAllocationIds = deviceKeyAllocations.Where(key => key.KeyAllocationId.HasValue).Select(key =>key.KeyAllocationId.Value).ToList();
            var cloudToDeviceKeyAllocationList = GetCloudToDeviceKeyAllocationRequest(deviceKeyAllocations, device, MessageType.IndirectKeyAllocation);

            var cloudToDeviceKeyAllocationMessageJson = JsonConvert.SerializeObject(cloudToDeviceKeyAllocationList, JsonHelper.GetJsonSerializerSettings());

            if (!await _iotHubServiceClient.IsDeviceOnline(device.DeviceName)) continue;

            await _iotHubServiceClient.SendIndirectMessageToDevice(device.DeviceName, cloudToDeviceKeyAllocationMessageJson);
            await _keyAllocationRepository.UpdateKeyUnsentAllocationStatus(deviceKeyAllocationIds, deviceId);
        }
    }

    public async Task ProcessDeviceKeyTransaction(KeyTransactionMessage keyTransactionMessage)
    {
        //using var transactionScope = new TransactionScope();

        var device = await _deviceRepository.GetDevice(keyTransactionMessage.DeviceId);

        foreach (var keyTransaction in keyTransactionMessage.KeyTransactions)
        {
            KeyAllocation keyAllocation = null;

            if (keyTransaction.KeyAllocationId.HasValue)
                keyAllocation = await _keyAllocationRepository.GetKeyAllocation(keyTransaction.KeyAllocationId.Value);

            await InsertKeyTransactionForFromDevice(keyAllocation?.KeyAllocationId, device, keyTransaction);

            var binId = keyTransaction?.BinId.HasValue ?? false ? keyTransaction.BinId : 
                    keyAllocation?.BinId.HasValue ?? false ? keyAllocation?.BinId: null;

            if (binId == null) continue;

            await _binRepository.UpdateBinInUse(binId.Value, GetBinInUseStatus(keyTransaction.KeyTransactionType));

            if (keyAllocation == null) continue;

            keyAllocation.Status = keyTransaction.KeyTransactionType;
            keyAllocation.BinId = keyTransaction.BinId;
           
            await _keyAllocationRepository.UpdateKeyAllocation(keyAllocation);
        }

        //transactionScope.Complete();
    }

    public async Task<KeyAllocation> GetKeyAllocation(Guid keyAllocationidId)
    {
        var keyAllocation = await _keyAllocationRepository.GetKeyAllocation(keyAllocationidId);

        return keyAllocation;
    }

    public Task<KeyAllocation> GetKeyAllocationHistory(string keyName)
    {
        throw new NotImplementedException();
    }

    public async Task ForceBinOpen(Guid deviceId, Guid binId)
    {
        var device = await _deviceRepository.GetDevice(deviceId);

        if (!await _iotHubServiceClient.IsDeviceOnline(device.DeviceName))
            throw new BadRequestException("Device is offline");

        var methodInvocation = new CloudToDeviceMethod(Constants.ForceBinOpenRequestHandler) { ResponseTimeout = TimeSpan.FromSeconds(20) };
        methodInvocation.SetPayloadJson(JsonConvert.SerializeObject(GetForceBinOpenRequest(device, binId), JsonHelper.GetJsonSerializerSettings()));
        var deviceToCloudResponse = await _iotHubServiceClient.SendDirectMessageToDevice(device.DeviceName, methodInvocation);

        if (deviceToCloudResponse?.Status != DeviceResponseStatus.Success)
            throw new Exception($"Force bin open failed: {deviceToCloudResponse}");

        var binOpenResponse = GetBinOpenResponse(deviceToCloudResponse.GetPayloadAsJson());

        //If the bin has current key allocation, transactiosn will come as key allocation transaction messages
        if (binOpenResponse.HasKeyAllocation) return;

        await _keyTransactionReposiotry.InsertKeyTransaction(new KeyTransaction()
        {
            BinId = binId,
            ChainId = device.ChainId,
            CreatedDateTime = DateTime.UtcNow,
            DeviceId = device.DeviceId,
            IsMessageSent = false,
            KeyAllocationId = null,
            KeyTransactionType = KeyAllocationStatus.BinForceOpened.ToString(),
            PropertyId = device.PropertyId
        });
    }
}
