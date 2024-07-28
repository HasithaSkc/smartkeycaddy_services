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
    private readonly IKeyAllocationRepository _keyRepository;
    private readonly IPropertyRoomRepository _propertyRoomRepository;

    public KeyAllocationService(ILogger<KeyAllocationService> logger,
        IIotHubServiceClient iotHubServiceClient,
        IDeviceRepository deviceRepository,
        IKeyAllocationRepository keyRepository,
        IPropertyRoomRepository propertyRoomRepository)
    {
        _logger = logger;
        _iotHubServiceClient = iotHubServiceClient;
        _deviceRepository = deviceRepository;
        _keyRepository = keyRepository;
        _propertyRoomRepository = propertyRoomRepository;
    }

    public async Task<KeyAllocationResponse> CreateKey(KeyAllocationRequest createKeyRequest)
    {
        var device = await _deviceRepository.GetDevice(createKeyRequest.DeviceId);

        if (device == null)
            throw new NotFoundException("Device not found");

        createKeyRequest.DeviceName = device.DeviceName;
        
        var keyAllocationResponse = new KeyAllocationResponse();
        var keysNotCreated = new List<KeyAllocation>();

        try
        {
            var keyAllocationList = await ConvertKeyAlloationRequestToDomainModel(createKeyRequest.KeyAllocation, device);

            if (!await _iotHubServiceClient.IsDeviceOnlineAsync(device.DeviceName))
            {
                _logger.LogError($"Device is offline. Creating keys on the server");
                await InsertKeyAllocationList(keyAllocationList);
                return ConvertToKeyAllocationResponse(keyAllocationList, device);
            }

            await InsertKeyAllocationList(keyAllocationList);

            // Invoke the direct method on the device
            var methodInvocation = new CloudToDeviceMethod(Constants.KeyAllocationRequestHandler) { ResponseTimeout = TimeSpan.FromSeconds(20) };
            methodInvocation.SetPayloadJson(JsonConvert.SerializeObject(GetCloudToDeviceKeyAllocationRequest(keyAllocationList, device), JsonHelper.GetJsonSerializerSettings()));
            var deviceToCloudResponse = await _iotHubServiceClient.SendMessageToDeviceAsync(device.DeviceName, methodInvocation);

            if (deviceToCloudResponse?.Status != DeviceResponseStatus.Success)
                throw new Exception("Key allocation failed");

            var deviceKeyAllocationResponse = GetDeviceKeyAllocationResponse(deviceToCloudResponse.GetPayloadAsJson());

            if (!(deviceKeyAllocationResponse?.KeyAllocation?.Any() ?? false))
                throw new Exception("Key allocation failed");

            foreach (var keyAllocation in keyAllocationList)
            {
                var allocatedKey = deviceKeyAllocationResponse.KeyAllocation.SingleOrDefault(keyResponse => string.Equals(keyAllocation.KeyName, keyResponse.KeyName, StringComparison.OrdinalIgnoreCase));
                keyAllocation.IsSuccessful = allocatedKey?.IsSuccessful ?? false;
                keyAllocation.Status = allocatedKey?.Status ?? string.Empty;

                if (!(allocatedKey?.IsSuccessful ?? false))
                {
                    keysNotCreated.Add(keyAllocation);
                    _logger.LogError($"Unable to create the requested key {keyAllocation.KeyName} on device. Status: {allocatedKey?.Status}");
                };

                await _keyRepository.UpdateKeyAllocation(keyAllocation);
            }

            return ConvertToKeyAllocationResponse(keyAllocationList, device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Key allocation failed");
            throw;
        }
        finally 
        { 
            await _iotHubServiceClient.CloseConnection(); 
        }
    }

    public async Task<bool> DeleteKey(Guid deviceId, Guid keyId)
    {
        var key  = await _keyRepository.GetKeyAllocation(deviceId, keyId);

        if (key == null)
            throw new Exception("Key not found");

        var device = await _deviceRepository.GetDevice(deviceId);

        var methodInvocation = new CloudToDeviceMethod(Constants.KeyDeleteRequestHandler) { ResponseTimeout = TimeSpan.FromSeconds(30) };
        methodInvocation.SetPayloadJson(JsonConvert.SerializeObject(new DeleteKeyRequest { KeyId = keyId, DeviceId = deviceId }));

        try
        {
            // Invoke the direct method on the device
            var response = await _iotHubServiceClient.SendMessageToDeviceAsync(device.DeviceName, methodInvocation);

            if (response == null || response.Status != DeviceResponseStatus.Success)
                throw new Exception("Failed to delete the key");
            
            await _keyRepository.Insertkey(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteKey");
            throw;
        }
        finally
        {
            await _iotHubServiceClient.CloseConnection();
        }

        return true;
    }

    public Task<bool> DeleteKey(Guid keyId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<KeyAllocation>> GetKeyAllocations(Guid deviceId)
    {
        return await _keyRepository.GetKeyAllocations(deviceId);
    }

    public async Task ProcessDeviceKeyTransaction(KeyTransactionMessage keyTransactionMessage)
    {
        foreach (var keyTransaction in keyTransactionMessage.KeyTransactions)
        {
            if (!keyTransaction.KeyAllocationId.HasValue) continue;

            var keyAllocation = await _keyRepository.GetKeyAllocation(keyTransaction.KeyAllocationId.Value);

            if (keyAllocation == null) continue;

            keyAllocation.Status = keyTransaction.Status;
            keyAllocation.IsSuccessful = keyTransaction.IsSuccessful;
            keyAllocation.BinId = keyTransaction.BinId;
            keyAllocation.KeyFobTagId = keyTransaction.KeyFobTagId;

            await _keyRepository.UpdateKeyAllocation(keyAllocation);

        }
    }
}


