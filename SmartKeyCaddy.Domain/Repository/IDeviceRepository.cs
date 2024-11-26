using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Repository;

public interface IDeviceRepository
{
    Task<List<Device>> GetDevices(Guid propertyId);
    Task<Device> GetDevice(Guid deviceId);
    Task<Device> GetDevice(Guid deviceId, string deviceName);
    Task<List<DeviceSetting>> GetDeviceSettings(Guid deviceId, Guid propertyId);
    Task RegisterDevice(Guid deviceId, bool isRegistered);
    Task<List<Bin>> GetDeviceBinDetails(Guid deviceId);
    Task<List<Bin>> GetDeviceBinDetailsWithKeyAllocation(Guid deviceId, DateTime localDateTime);
}
