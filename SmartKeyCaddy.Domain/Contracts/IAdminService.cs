
using SmartKeyCaddy.Models;
using SmartKeyCaddy.Models.Messages;

namespace SmartKeyCaddy.Domain.Contracts;

public interface IAdminService
{
    Task RegisterDevice(RegisterDeviceMessage registerDeviceMessage);
    Task<DeviceConfigurationMessage> GetDeviceConfigurationMessage(Guid deviceId);
}
