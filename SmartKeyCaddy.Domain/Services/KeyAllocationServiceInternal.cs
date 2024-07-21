using Azure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Common.JsonHelper;
using SmartKeyCaddy.Models;
using SmartKeyCaddy.Models.Messages;
using System.Text.RegularExpressions;

namespace SmartKeyCaddy.Domain.Services;

public partial class KeyAllocationService
{
    private async Task<List<KeyAllocation>> ConvertKeyAlloationRequestToDomainModel(List<KeyAllocationItem> keyAllocationRequestItemList, Device device)
    {
        var keyAllocationList = new List<KeyAllocation>();
        var propertyRooms = await _propertyRoomRepository.GetPropertyRooms(device.PropertyId);

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
            KeyFobTagId = propertyRoom?.KeyFobTagId,
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
                Status = keyAllocationItem.Status
            });
        }

        return keyAllocationResponse;
    }

    private async Task InsertKeyAllocationList(List<KeyAllocation> keyAllocation)
    {
        foreach (var keyAllocationItem in keyAllocation)
        {
            await InsertKeyAllocation(keyAllocationItem);
        }
    }

    private async Task InsertKeyAllocation(KeyAllocation keyAllocation)
    {
        if (await _keyRepository.KeyExists(keyAllocation.KeyName, keyAllocation.CheckInDate))
        {
            keyAllocation.Status = KeyAllocationStatus.KeyExistsOnServer.ToString();
            keyAllocation.IsSuccessful = false;
            _logger.LogInformation($"Key: {keyAllocation.KeyName} Checkin date: {keyAllocation.CheckInDate} already exists");
            return;
        }

        keyAllocation.Status = keyAllocation.Status ?? KeyAllocationStatus.KeyCreated.ToString();
        keyAllocation.IsSuccessful = true;
        await _keyRepository.Insertkey(keyAllocation);
    }

    private DeviceKeyAllocationRequest GetCloudToDeviceKeyAllocationRequest(List<KeyAllocation> keyAllocationList, Device device)
    {
        var deviceKeyAllocationRequest = new DeviceKeyAllocationRequest()
        {
            DeviceName = device.DeviceName,
            DeviceId = device.DeviceId,
            KeyAllocation = new List<DeviceKeyAllocationItem>()
        };

        foreach (var keyAllocation in keyAllocationList)
        {
            deviceKeyAllocationRequest.KeyAllocation.Add(new DeviceKeyAllocationItem()
            {
                KeyAllocationId = keyAllocation.KeyAllocationId,
                CheckInDate = keyAllocation.CheckInDate?.ToString(Constants.ShortDateString) ?? string.Empty,
                CheckOutDate = keyAllocation.CheckOutDate?.ToString(Constants.ShortDateString) ?? string.Empty,
                GuestWelcomeMessage = keyAllocation.GuestWelcomeMessage,
                KeyPickupInstruction = keyAllocation.KeyPickupInstruction,
                KeyName = keyAllocation.KeyName,
                KeyPinCode = keyAllocation.KeyPinCode,
                KeyFobTagId = keyAllocation.KeyFobTagId
            });
        }

        return deviceKeyAllocationRequest;
    }

    private KeyAllocation ConvertKeyTransactionMessageToDomainModel(KeyTransactionMessage keyTransactionMessage)
    {
        return new KeyAllocation
        {
            KeyAllocationId = keyTransactionMessage.KeyAllocationId ?? Guid.NewGuid(),
            KeyName = keyTransactionMessage.KeyName,
            KeyPinCode = keyTransactionMessage.KeyPinCode,
            DeviceId = keyTransactionMessage.DeviceId,
            KeyFobTagId = keyTransactionMessage.KeyFobTagId,
            BinId = keyTransactionMessage.BinId,
            Status = keyTransactionMessage.Status,
            IsSuccessful = keyTransactionMessage.IsSuccessful
        };
    }
}
