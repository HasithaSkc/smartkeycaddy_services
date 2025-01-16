using Microsoft.Extensions.Logging;
using SmartKeyCaddy.Models;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models.Exceptions;
using Microsoft.Azure.Devices;
using Device = SmartKeyCaddy.Models.Device;
using System.Text;

namespace SmartKeyCaddy.Domain.Services;

public partial class DeviceService : IDeviceService
{
    private readonly ILogger<IDeviceService> _logger;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IIotHubServiceClient _iotHubServiceClient;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IStorageContainerService _storageContainerService;

    public DeviceService(
        ILogger<IDeviceService> logger,
        IDeviceRepository deviceRepository,
        IPropertyRepository propertyRepository,
        IIotHubServiceClient iotHubServiceClient,
        IStorageContainerService storageContainerService)
    {
        _logger = logger;
        _deviceRepository = deviceRepository;
        _propertyRepository = propertyRepository;
        _iotHubServiceClient = iotHubServiceClient;
        _storageContainerService = storageContainerService;
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
        var device = await _deviceRepository.GetDevice(deviceId);

        if (device == null)
            throw new NotFoundException("Device not found");

        var property = await _propertyRepository.GetProperty(device.PropertyId);

        if (device == null)
            throw new NotFoundException("Property not found");

        var localDateTimeNow = CommonFunctions.ConvertToLocalDateTime(DateTime.UtcNow, property.TimeZone);

        return await _deviceRepository.GetDeviceBinDetailsWithKeyAllocation(deviceId, localDateTimeNow);
    }

    public Task<Guid> UpdateDevice(Device device)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> GetDeviceOnlineStatus(Guid deviceId)
    {
        var device = await _deviceRepository.GetDevice(deviceId);

        if (device == null)
            throw new NotFoundException("Device not found");

        return await _iotHubServiceClient.IsDeviceOnline(device.DeviceName);
    }

    public async Task<Tuple<byte[],string>> GetDeviceLog(Guid deviceId)
    {
        try
        {
            var device = await _deviceRepository.GetDevice(deviceId);

            if (device == null)
                throw new NotFoundException();

            var deviceName = device.DeviceName;

            if (!await _iotHubServiceClient.IsDeviceOnline(device.DeviceName))
                throw new DeviceOfflineException();

            // Invoke the direct method on the device
            var methodInvocation = new CloudToDeviceMethod(Constants.DeviceLogRequestHandler) { ResponseTimeout = TimeSpan.FromSeconds(20) };
            var deviceToCloudResponse = await _iotHubServiceClient.SendDirectMessageToDevice(device.DeviceName, methodInvocation);

            if (deviceToCloudResponse?.Status != DeviceResponseStatus.Success)
                throw new Exception("Get device log failed");

            var success = ServiceHelper.DeviceLogResponse(deviceToCloudResponse.GetPayloadAsJson());

            if (!success)
                throw new Exception("Device log download failed");

            var fileContent = await _storageContainerService.GetBlob(Constants.DeviceLogStorageContainerName, $"{device.DeviceName}.txt");

            return new Tuple<byte[], string>(fileContent, deviceName); 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get device log failed");
            throw;
        }
    }
}
