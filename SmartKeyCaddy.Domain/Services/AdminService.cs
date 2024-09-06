using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Common.JsonHelper;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models.Messages;

namespace SmartKeyCaddy.Domain.Services;

public partial class AdminService : IAdminService
{
    private readonly ILogger<KeyAllocationService> _logger;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IBinRepository _binRepository;
    private readonly IKeyFobTagRepository _keyFobTagRepository;
    private readonly IotHubServiceClient _iotHubServiceClient;

    public AdminService(ILogger<KeyAllocationService> logger,
        IDeviceRepository deviceRepository,
        IBinRepository binRepository,
        IKeyFobTagRepository keyFobTagRepository,
        IotHubServiceClient iotHubServiceClient)
    {
        _logger = logger;
        _deviceRepository = deviceRepository;
        _binRepository = binRepository;
        _keyFobTagRepository = keyFobTagRepository;
        _iotHubServiceClient = iotHubServiceClient;
    }

    public async Task RegisterDevice(DeviceRegisterMessage registerDeviceMessage)
    {
        var device = await _deviceRepository.GetDevice(registerDeviceMessage.DeviceId);
        _logger.LogInformation($"Regitering device: {device.DeviceName}");

        //if (device.IsRegistered) throw new Exception("Device already registered");

        await _deviceRepository.RegisterDevice(registerDeviceMessage.DeviceId, true);

        var deviceConfigurationMessage = await GetDeviceConfigurationMessage(device);
        var deviceConfigurationMessageJson = JsonConvert.SerializeObject(deviceConfigurationMessage, JsonHelper.GetJsonSerializerSettings());

        await _iotHubServiceClient.SendIndirectMessageToDevice(device.DeviceName, deviceConfigurationMessageJson);
    }

    public async Task DisableBin(Guid deviceId, Guid binId)
    {
        var device = await _deviceRepository.GetDevice(deviceId);

        var deviceConfigurationMessageJson = JsonConvert.SerializeObject(new DisableBinMessage()
        {
            BinId = binId,
            DeviceId = deviceId
        }, JsonHelper.GetJsonSerializerSettings());

        await _iotHubServiceClient.SendIndirectMessageToDevice(device.DeviceName, deviceConfigurationMessageJson);
    }
}