
using SmartKeyCaddy.Models.Messages;

namespace SmartKeyCaddy.Domain.Contracts;

public interface IAdminService
{
    Task RegisterDevice(DeviceRegisterMessage registerDeviceMessage);
    Task DisableBin(Guid deviceId, Guid binId);
}
