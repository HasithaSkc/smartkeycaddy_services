
using SmartKeyCaddy.Models.Messages;

namespace SmartKeyCaddy.Domain.Contracts;

public interface IAdminService
{
    Task RegisterDevice(DeviceRegisterMessage registerDeviceMessage);
    Task DisableBin(Guid deviceId, Guid binId);
    Task<string> GenerateOfflinePinCode(Guid deviceId, string roomNumber);
}
