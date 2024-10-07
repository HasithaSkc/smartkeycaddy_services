using SmartKeyCaddy.Models;
using SmartKeyCaddy.Models.Configurations;

namespace SmartKeyCaddy.Domain.Repository
{
    public interface IDeviceRepository
    {
        Task<List<Device>> GetDevices(Guid locationId);
        Task<Device> GetDevice(Guid deviceId);
        Task<Device> GetDevice(Guid deviceId, string deviceName);
        Task<List<DeviceSetting>> GetDeviceSettings(Guid deviceId, Guid propertyId);
        Task RegisterDevice(Guid deviceId, bool isRegistered); 
    }
}
