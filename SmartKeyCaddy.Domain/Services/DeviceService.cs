using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Azure.Communication.Email;
using HotelCheckIn.Domain.Contracts;
using HotelCheckIn.Models.Configurations;
using SmartKeyCaddy.Models;
using SmartKeyCaddy.Models.Configurations;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Domain.Repository;

namespace SmartKeyCaddy.Domain.Services;

public partial class DeviceService : IDeviceService
{
    private readonly ILogger<IDeviceService> _logger;
    private readonly IDeviceRepository _deviceRepository;
    public DeviceService(
        ILogger<IDeviceService> logger,
        IDeviceRepository deviceRepository)
    {
        _logger = logger;
        _deviceRepository = deviceRepository;
    }

    public async Task<List<Device>> GetDevices(Guid propertyId)
    {
        return await _deviceRepository.GetDevices(propertyId);
    }

    public async Task<Device> GetDevice(Guid deviceId)
    {
        return await _deviceRepository.GetDevice(deviceId);
    }

    public Task<Guid> AddDevice(Device device)
    {
        throw new NotImplementedException();
    }

    public Task<Property> DeleteDevice(Guid deviceId)
    {
        throw new NotImplementedException();
    }

    
    public async Task<List<Bin>> GetDeviceBinDetails(Guid deviceId)
    {
        return await _deviceRepository.GetDeviceBinDetails(deviceId);
    }

    public Task<Guid> UpdateDevice(Device device)
    {
        throw new NotImplementedException();
    }
}
