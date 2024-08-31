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
using System.Transactions;

namespace SmartKeyCaddy.Domain.Services;

public partial class KeyAllocationService : IKeyAllocationService
{
    private readonly ILogger<KeyAllocationService> _logger;
    private readonly IIotHubServiceClient _iotHubServiceClient;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IKeyAllocationRepository _keyAllocationRepository;
    private readonly IPropertyRoomRepository _propertyRoomRepository;
    private readonly IBinRepository _binRepository;

    public KeyAllocationService(ILogger<KeyAllocationService> logger,
        IIotHubServiceClient iotHubServiceClient,
        IDeviceRepository deviceRepository,
        IKeyAllocationRepository keyRepository,
        IPropertyRoomRepository propertyRoomRepository,
        IBinRepository binRepository)
    {
        _logger = logger;
        _iotHubServiceClient = iotHubServiceClient;
        _deviceRepository = deviceRepository;
        _keyAllocationRepository = keyRepository;
        _propertyRoomRepository = propertyRoomRepository;
        _binRepository = binRepository;
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
            var keyAllocationList = await ConvertKeyAlloationRequestToDomainModel(keyAllocationRequest.KeyAllocation, device);

            if (!await _iotHubServiceClient.IsDeviceOnline(device.DeviceName))
            {
                _logger.LogError($"Device is offline. Creating keys on the server");
                await InsertOrUpdateKeyAllocationList(keyAllocationList);
                return ConvertToKeyAllocationResponse(keyAllocationList, device);
            }

            await ValidateKeyAllocationList(keyAllocationList);

            // Invoke the direct method on the device
            var methodInvocation = new CloudToDeviceMethod(Constants.KeyAllocationRequestHandler) { ResponseTimeout = TimeSpan.FromSeconds(20) };
            methodInvocation.SetPayloadJson(JsonConvert.SerializeObject(GetCloudToDeviceKeyAllocationRequest(keyAllocationList, device), JsonHelper.GetJsonSerializerSettings()));
            var deviceToCloudResponse = await _iotHubServiceClient.SendDirectMessageToDevice(device.DeviceName, methodInvocation);

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

                await InsertOrUpdateKeyAllocation(keyAllocation);
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
        return await _keyAllocationRepository.GetKeyAllocations(deviceId);
    }

    public async Task ProcessDeviceKeyTransaction(KeyTransactionMessage keyTransactionMessage)
    {
        //using var transactionScope = new TransactionScope();

        foreach (var keyTransaction in keyTransactionMessage.KeyTransactions)
        {
            if (!keyTransaction.KeyAllocationId.HasValue) continue;

            var keyAllocation = await _keyAllocationRepository.GetKeyAllocation(keyTransaction.KeyAllocationId.Value);

            if (!(keyTransaction?.BinId.HasValue ?? false)) continue;

            keyAllocation.Status = keyTransaction.Status;
            keyAllocation.IsSuccessful = keyTransaction.IsSuccessful;
            keyAllocation.BinId = keyTransaction.BinId;
            keyAllocation.KeyFobTagId = keyTransaction.KeyFobTagId;

            await _keyAllocationRepository.UpdateKeyAllocation(keyAllocation);

            await _binRepository.UpdateBinInUse(keyTransaction.BinId.Value, GetBinInUse(keyTransaction.Status));
        }

        //transactionScope.Complete();
    }

    private bool GetBinInUse(string status)
    { 
        var keyAllocationStatus = Enum.Parse(typeof(KeyAllocationStatus), status);

        return keyAllocationStatus switch
        {
            KeyAllocationStatus.KeyLoaded => true,
            KeyAllocationStatus.KeyPickedUp => false,
            _ => false,
        };
    }
}


