using SmartKeyCaddy.Common;
using SmartKeyCaddy.Models;
using SmartKeyCaddy.Models.Messages;

namespace SmartKeyCaddy.Domain.Services;

public partial class AdminService
{
    private async Task<DeviceConfigurationMessage> GetDeviceConfigurationMessage(Device device)
    {
        var bins = await _binRepository.GetBins(device.DeviceId);
        var keyFobTags = await _keyFobTagRepository.GetKeyFobTags(device.PropertyId);
        var deviceSettings = await _deviceRepository.GetDeviceSettings(device.DeviceId, device.PropertyId);
        var property = await _propertyRepository.GetProperty(device.PropertyId);

        var deviceConfiguratioNMessage = new DeviceConfigurationMessage()
        {
            MessageType = MessageType.DeviceConfiguration.ToString(),
            DeviceId = device.DeviceId,
            DeviceName = device.DeviceName,
            Bins = bins,
            KeyFobTags = keyFobTags,
            DeviceSettings = deviceSettings,
            Property = property
        };

        return deviceConfiguratioNMessage;
    }
}


