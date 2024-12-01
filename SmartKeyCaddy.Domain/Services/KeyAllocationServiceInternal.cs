using Azure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Common.JsonHelper;
using SmartKeyCaddy.Models;
using System.Text.RegularExpressions;

namespace SmartKeyCaddy.Domain.Services;

public partial class KeyAllocationService
{
    private async Task<List<KeyAllocation>> EnrichKeyAllocationRequest(List<KeyAllocationItem> keyAllocationRequestItemList, Device device)
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

    private KeyAllocation ConvertToDomainModel(KeyAllocationItem allocatedKey, Device device, PropertyRoom propertyRoom)
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
            KeyFobTagId = propertyRoom?.KeyFobTag?.KeyFobTagId,
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

    private DeviceKeyAllocationResponse GetDeviceKeyAllocationResponse(string responseJson)
    {
        var formattedJson = Regex.Unescape(responseJson);

        if (formattedJson.StartsWith("\"") && formattedJson.EndsWith("\""))
            formattedJson = formattedJson.Substring(1, formattedJson.Length - 2);

        return JsonConvert.DeserializeObject<DeviceKeyAllocationResponse>(formattedJson, JsonHelper.GetJsonSerializerSettings());
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
        var existingKeyAllocation = await _keyAllocationRepository.GetKeyAllocationByKeyName(keyAllocation.KeyName);

        ValidateKeyAllocation(keyAllocation, existingKeyAllocation);

        if (!keyAllocation.IsSuccessful) return;

        if (existingKeyAllocation !=null)
             await _keyAllocationRepository.UpdateKeyAllocation(keyAllocation);
        else
            await _keyAllocationRepository.InsertkeyAllocation(keyAllocation);
    }

    private async Task ValidateKeyAllocationList(List<KeyAllocation> keyAllocationList)
    {
        foreach (var keyAllocation in keyAllocationList)
        {
            var existingKeyAllocation = await _keyAllocationRepository.GetKeyAllocationByKeyName(keyAllocation.KeyName);

            ValidateKeyAllocation(keyAllocation, existingKeyAllocation);
        }
    }

    private void ValidateKeyAllocation(KeyAllocation keyAllocation, KeyAllocation existingKeyAllocation)
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

            //If key already loaded then don't create
            if (string.Equals(existingKeyAllocation?.Status, KeyAllocationStatus.KeyLoaded.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                keyAllocation.Status = KeyAllocationErrorStatus.KeyAlreadyExists.ToString();
                keyAllocation.IsSuccessful = false;
                keyAllocation.KeyPinCode = string.Empty;
                _logger.LogInformation($"Key: {keyAllocation.KeyName} with status: {keyAllocation.Status} already exists");
            }
            else
            {
                keyAllocation.IsSuccessful = true;
                keyAllocation.Status = KeyAllocationStatus.KeyCreated.ToString();
            }

            return;
        }

        keyAllocation.Status = KeyAllocationStatus.KeyCreated.ToString();
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

        foreach (var keyAllocation in keyAllocationList.Where(keyAloc=>keyAloc.IsSuccessful))
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
                KeyFobTagId = keyAllocation.KeyFobTagId
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
}