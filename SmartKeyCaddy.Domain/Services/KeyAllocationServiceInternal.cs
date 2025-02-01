using Microsoft.Azure.Devices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Common.JsonHelper;
using SmartKeyCaddy.Models;
using Device = SmartKeyCaddy.Models.Device;

namespace SmartKeyCaddy.Domain.Services;

public partial class KeyAllocationService
{
    private async Task<List<KeyAllocation>> PrepareKeyAllocationRequest(List<KeyAllocationItem> keyAllocationRequestItemList, Device device,
         KeyAllocationType keyAllocationType)
    {
        switch (keyAllocationType)
        {
            case KeyAllocationType.Managed:
                return await PrepareManagedKeyAllocationRequest(keyAllocationRequestItemList, device);
            case KeyAllocationType.SelfManaged:
                return await PrepareSelfManagedKeyAllocationRequest(keyAllocationRequestItemList, device);
            default:
                return new List<KeyAllocation>();
        }
    }

    private async Task<List<KeyAllocation>> PrepareManagedKeyAllocationRequest(List<KeyAllocationItem> keyAllocationRequestItemList, Device device)
    {
        var keyAllocationList = new List<KeyAllocation>();
        var propertyRooms = await _propertyRoomRepository.GetPropertyRoomKeyFobTags(device.PropertyId);

        foreach (var keyAllocationRequestItem in keyAllocationRequestItemList)
        {
            var propertyRoom = propertyRooms.SingleOrDefault(room => string.Equals(room.RoomNumber, keyAllocationRequestItem.RoomNumber, StringComparison.OrdinalIgnoreCase));
            keyAllocationList.Add(ConvertToDomainModel(keyAllocationRequestItem, device, propertyRoom));
        }

        return keyAllocationList;
    }

    private async Task<List<KeyAllocation>> PrepareSelfManagedKeyAllocationRequest(List<KeyAllocationItem> keyAllocationRequestItemList, Device device)
    {
        var keyAllocationList = new List<KeyAllocation>();
        var propertyRooms = await _propertyRoomRepository.GetPropertyRoomKeyFobTags(device.PropertyId);

        foreach (var keyAllocationRequestItem in keyAllocationRequestItemList)
        {
            var selfManagedKeyAllocation = await _keyAllocationRepository.GetSelfManagedKeyAllocation(device.PropertyId, keyAllocationRequestItem.RoomNumber);

            var propertyRoom = propertyRooms.SingleOrDefault(room => string.Equals(room.RoomNumber, keyAllocationRequestItem.RoomNumber, StringComparison.OrdinalIgnoreCase));
            keyAllocationList.Add(ConvertToDomainModel(keyAllocationRequestItem, device, propertyRoom, selfManagedKeyAllocation));
        }

        return keyAllocationList;
    }

    private KeyAllocation ConvertToDomainModel(KeyAllocationItem allocatedKey, Device device, PropertyRoom propertyRoom, KeyAllocation selfManagedKeyAllocation = null)
    {
        return new KeyAllocation
        {
            KeyAllocationId = allocatedKey.KeyAllocationId ?? Guid.NewGuid(),
            KeyName = allocatedKey.KeyName,
            KeyPinCode = allocatedKey.KeyPinCode,
            GuestWelcomeMessage = allocatedKey.GuestWelcomeMessage,
            KeyPickupInstruction = allocatedKey.KeyPickupInstruction,
            CheckInDate = string.IsNullOrEmpty(allocatedKey.CheckInDate) ? null : Convert.ToDateTime(allocatedKey.CheckInDate),
            CheckOutDate = string.IsNullOrEmpty(allocatedKey.CheckOutDate) ? null : Convert.ToDateTime(allocatedKey.CheckOutDate),
            KeyFobTagId = selfManagedKeyAllocation != null ? selfManagedKeyAllocation.KeyFobTagId : propertyRoom?.KeyFobTag?.KeyFobTagId,
            BinId = selfManagedKeyAllocation != null ? selfManagedKeyAllocation.BinId : null,
            Status = GetKeyAllocationStatus(KeyAllocationType.SelfManaged, selfManagedKeyAllocation),
            DeviceId = device.DeviceId,
            ChainId = device.ChainId,
            PropertyId = device.PropertyId
        };
    }

    /// <summary>
    /// TODO: Extend the logic to handle multiple devices in the sme location. 
    /// </summary>
    /// <param name="devices"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task<Device> GetDevice(Guid locationId)
    {
        var devices = await _deviceRepository.GetDevices(locationId);

        if (!(devices?.Any() ?? false))
            throw new Exception("No devices found");
        return devices.First();
    }

    private KeyAllocationResponse ConvertToKeyAllocationResponse(List<KeyAllocation> keyAllocation, Device device)
    {
        var keyAllocationResponse = new KeyAllocationResponse()
        {
            DeviceName = device.DeviceName,
            DeviceId = device.DeviceId,
            KeyAllocation = new List<KeyAllocationResponseItem>()
        };

        foreach (var keyAllocationItem in keyAllocation)
        {
            keyAllocationResponse.KeyAllocation.Add(new KeyAllocationResponseItem()
            {
                KeyAllocationId = keyAllocationItem.KeyAllocationId,
                CheckInDate = keyAllocationItem.CheckInDate?.ToString(Constants.ShortDateString) ?? string.Empty,
                CheckOutDate = keyAllocationItem.CheckOutDate?.ToString(Constants.ShortDateString) ?? string.Empty,
                GuestWelcomeMessage = keyAllocationItem.GuestWelcomeMessage,
                KeyPickupInstruction = keyAllocationItem.KeyPickupInstruction,
                IsSuccessful = keyAllocationItem.IsSuccessful,
                KeyName = keyAllocationItem.KeyName,
                KeyPinCode = keyAllocationItem.KeyPinCode,
                Status = keyAllocationItem.Status,
            });
        }

        return keyAllocationResponse;
    }

    private async Task InsertOrUpdateKeyAllocationList(List<KeyAllocation> keyAllocationList)
    {
        foreach (var keyAllocation in keyAllocationList)
        {
            await InsertOrUpdateKeyAllocation(keyAllocation);
        }
    }

    private async Task InsertOrUpdateKeyAllocation(KeyAllocation keyAllocation)
    {
        var existingKeyAllocation = await _keyAllocationRepository.GetKeyAllocationByKeyName(keyAllocation.KeyName, keyAllocation.PropertyId);

        ValidateKeyAllocation(keyAllocation, existingKeyAllocation);

        if (!keyAllocation.IsSuccessful) return;

        if (existingKeyAllocation != null)
            await _keyAllocationRepository.UpdateKeyAllocation(keyAllocation);
        else
            await _keyAllocationRepository.InsertkeyAllocation(keyAllocation);
    }

    private async Task ValidateKeyAllocationList(List<KeyAllocation> keyAllocationList, Device device)
    {
        if (!(keyAllocationList?.Any() ?? false))
            return;

        await GenerateKeyPincodeIfNotExists(keyAllocationList, device);

        foreach (var keyAllocation in keyAllocationList)
        {
            var existingKeyAllocation = await _keyAllocationRepository.GetKeyAllocationByKeyName(keyAllocation.KeyName, device.PropertyId);

            ValidateKeyAllocation(keyAllocation, existingKeyAllocation);
        }
    }

    private async void ValidateKeyAllocation(KeyAllocation keyAllocation, KeyAllocation existingKeyAllocation)
    {
        if (keyAllocation.KeyFobTagId == null)
        {
            keyAllocation.Status = KeyAllocationErrorStatus.KeyFobNotFound.ToString();
            keyAllocation.IsSuccessful = false;
            return;
        }

        if (existingKeyAllocation != null)
        {
            keyAllocation.KeyAllocationId = existingKeyAllocation.KeyAllocationId;
            var exitingKeyAllocationStatus = EnumExtensions.GetEnumValue(existingKeyAllocation?.Status ?? string.Empty, KeyAllocationStatus.KeyCreated);

            //If key already loaded or dropped off then don't allow to send to the device
            if (exitingKeyAllocationStatus == KeyAllocationStatus.KeyLoaded || exitingKeyAllocationStatus == KeyAllocationStatus.KeyDroppedOff)
            {
                keyAllocation.IsSuccessful = false;
                keyAllocation.Status = exitingKeyAllocationStatus == KeyAllocationStatus.KeyLoaded? KeyAllocationErrorStatus.KeyAlreadyLoaded.ToString()
                    : KeyAllocationErrorStatus.KeyAlreadyDroppedOff.ToString();
                _logger.LogInformation($"Key: {keyAllocation.KeyName} with status: {keyAllocation.Status} already exists");
            }
            else
                keyAllocation.IsSuccessful = true;
            return;
        }

        
        keyAllocation.IsSuccessful = true;
    }

    private DeviceKeyAllocationRequest GetCloudToDeviceKeyAllocationRequest(List<KeyAllocation> keyAllocationList, Device device, MessageType messageType)
    {
        var deviceKeyAllocationRequest = new DeviceKeyAllocationRequest()
        {
            DeviceName = device.DeviceName,
            DeviceId = device.DeviceId,
            KeyAllocation = new List<DeviceKeyAllocationItem>(),
            MessageType = messageType.ToString()
        };

        foreach (var keyAllocation in keyAllocationList.Where(keyAloc => keyAloc.IsSuccessful))
        {
            deviceKeyAllocationRequest.KeyAllocation.Add(new DeviceKeyAllocationItem()
            {
                KeyAllocationId = keyAllocation.KeyAllocationId,
                CheckInDate = keyAllocation.CheckInDate?.ToString(Constants.IsoDateString) ?? string.Empty,
                CheckOutDate = keyAllocation.CheckOutDate?.ToString(Constants.IsoDateString) ?? string.Empty,
                GuestWelcomeMessage = keyAllocation.GuestWelcomeMessage,
                KeyPickupInstruction = keyAllocation.KeyPickupInstruction,
                KeyName = keyAllocation.KeyName,
                KeyPinCode = keyAllocation.KeyPinCode,
                KeyFobTagId = keyAllocation.KeyFobTagId,
                Status = keyAllocation.Status,
                BinId = keyAllocation.BinId
            });
        }

        return deviceKeyAllocationRequest;
    }

    private async Task InsertKeyTransactionForFromDevice(Guid? keyAllocationId, Device device, KeyTransaction keyTransaction)
    {
        keyTransaction.BinId = keyTransaction.BinId;
        keyTransaction.KeyTransactionType = keyTransaction.KeyTransactionType;
        keyTransaction.KeyAllocationId = keyAllocationId;
        keyTransaction.ChainId = device.ChainId;
        keyTransaction.PropertyId = device.PropertyId;
        keyTransaction.DeviceId = device.DeviceId;

        await _keyTransactionReposiotry.InsertKeyTransaction(keyTransaction);
    }

    private bool GetBinInUseStatus(string status)
    {
        var keyAllocationStatus = Enum.Parse(typeof(KeyAllocationStatus), status);

        return keyAllocationStatus switch
        {
            KeyAllocationStatus.KeyLoaded => true,
            KeyAllocationStatus.KeyPickedUp => false,
            KeyAllocationStatus.KeyDroppedOff => true,
            _ => false,
        };
    }

    private ForceBinOpenRequest GetForceBinOpenRequest(Device device, Guid binId)
    {
        return new ForceBinOpenRequest()
        {
            DeviceName = device.DeviceName,
            DeviceId = device.DeviceId,
            MessageType = MessageType.ForceBinOpen.ToString(),
            BinId = binId
        };
    }

    private BinOpenResponse GetBinOpenResponse(string responseJson)
    {
        var formattedJson = JsonHelper.FormatDeviceResponse(responseJson);

        return JsonConvert.DeserializeObject<BinOpenResponse>(formattedJson, JsonHelper.GetJsonSerializerSettings());
    }

    private async Task<KeyAllocationType> GetKeyAllocationType(Device device)
    {
        var deviceSetting = await _deviceRepository.GetDeviceSetting(device.DeviceId, Constants.KeyAllocationType);

        return (!Enum.TryParse<KeyAllocationType>(deviceSetting?.SettingValue ?? string.Empty, true, out var keyAllocationType)) ? 
            KeyAllocationType.Managed : keyAllocationType;
    }

    private string GetKeyAllocationStatus(KeyAllocationType keyAllocationType, KeyAllocation? keyAllocation)
    {
        //If Self managed, check whether you have the binid and status = KeyDroppedOff.
        //If not means that this device is in self managed mode and creating a brand new key
        var keyAllocationStatus = keyAllocationType == KeyAllocationType.SelfManaged ?
            (keyAllocation?.BinId != null && string.Equals(keyAllocation.Status, KeyAllocationStatus.KeyDroppedOff.ToString(), StringComparison.OrdinalIgnoreCase)) ?
            KeyAllocationStatus.KeyLoaded : KeyAllocationStatus.KeyCreated : KeyAllocationStatus.KeyCreated;

        return keyAllocationStatus.ToString();
    }

    private async Task SyncKeyAllocationListWithDeviceResponse(List<KeyAllocation> keyAllocationList, DeviceKeyAllocationResponse deviceKeyAllocationResponse)
    {
        foreach (var keyAllocation in keyAllocationList)
        {
            var allocatedKey = deviceKeyAllocationResponse.KeyAllocation.SingleOrDefault(keyResponse => string.Equals(keyAllocation.KeyName, keyResponse.KeyName, StringComparison.OrdinalIgnoreCase));
            keyAllocation.IsSuccessful = allocatedKey?.IsSuccessful ?? false;
            keyAllocation.Status = allocatedKey?.Status ?? string.Empty;
            keyAllocation.IsMessageSent = keyAllocation.IsSuccessful;

            if (!(allocatedKey?.IsSuccessful ?? false))
            {
                _logger.LogError($"Unable to create the requested key {keyAllocation.KeyName} on device. Status: {allocatedKey?.Status}.");
            };

            await InsertOrUpdateKeyAllocation(keyAllocation);
        }
    }

    private async Task GenerateKeyPincodeIfNotExists(List<KeyAllocation> keyAllocationList, Device device)
    {
        if (!keyAllocationList.Any(key => string.IsNullOrEmpty(key.KeyPinCode)))
            return;

        var chekinDate = keyAllocationList.Where(res => res.CheckInDate.HasValue).Min(res => res.CheckInDate);

        if (chekinDate == null) return;

        var exitingKeyPincodes = await _keyAllocationRepository.GetExistingKeyPincodes(device.DeviceId, chekinDate.Value);
        exitingKeyPincodes = exitingKeyPincodes ?? new List<string>();

        foreach (var keyAllocation in keyAllocationList)
        {
            var keyPinCode = CommonFunctions.GenerateRandomKeyCode(exitingKeyPincodes, Constants.SmartKeyCaddyKeyPinCodeNoOfDigits);
            exitingKeyPincodes.Add(keyPinCode);
        }
    }
}