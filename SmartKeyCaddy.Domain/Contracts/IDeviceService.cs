﻿using Microsoft.Azure.Devices;
using SmartKeyCaddy.Models;
using Device = SmartKeyCaddy.Models.Device;

namespace SmartKeyCaddy.Domain.Contracts;
public interface IDeviceService
{
    Task<List<Device>> GetDevices(Guid propertyId);
    Task<Device> GetDevice(Guid deviceId);
    Task<Guid> AddDevice(Device device);
    Task<Guid> UpdateDevice(Device device);
    Task<Property> DeleteDevice(Guid deviceId);
    Task<List<Bin>> GetDeviceBinDetails(Guid deviceId);
}
