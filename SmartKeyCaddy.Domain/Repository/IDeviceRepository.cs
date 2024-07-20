using SmartKeyCaddy.Models;
using SmartKeyCaddy.Models.Configurations;

namespace SmartKeyCaddy.Domain.Repository
{
    public interface IDeviceRepository
    {
        Task<List<Device>> GetDevices(Guid locationId);
        Task<Device> GetDevice(Guid deviceId);
    }
}
