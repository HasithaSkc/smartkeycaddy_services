using Microsoft.Azure.Devices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Common.JsonHelper;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;
using SmartKeyCaddy.Models.Exceptions;
using SmartKeyCaddy.Models.Messages;

namespace SmartKeyCaddy.Domain.Services;

public partial class AdminService : IAdminService
{
    private readonly ILogger<KeyAllocationService> _logger;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IBinRepository _binRepository;
    private readonly IKeyFobTagRepository _keyFobTagRepository;
    private readonly IServiceBusPublisherService _serviceBusPublisherService;

    public AdminService(ILogger<KeyAllocationService> logger,
        IDeviceRepository deviceRepository,
        IBinRepository binRepository,
        IKeyFobTagRepository keyFobTagRepository,
        IServiceBusPublisherService serviceBusPublisherService)
    {
        _logger = logger;
        _deviceRepository = deviceRepository;
        _binRepository = binRepository;
        _keyFobTagRepository = keyFobTagRepository;
        _serviceBusPublisherService = serviceBusPublisherService;
    }

    public async Task RegisterDevice(RegisterDeviceMessage registerDeviceMessage)
    {
        var device = await _deviceRepository.GetDevice(registerDeviceMessage.DeviceId);

        if (device.IsRegistered) throw new Exception("Device already registered");

        await _deviceRepository.RegisterDevice(registerDeviceMessage.DeviceId, true);

        var deviceConfiguratioNMessage = await GetDeviceConfigurationMessage(registerDeviceMessage.DeviceId);

        await _serviceBusPublisherService.PublishMessage(deviceConfiguratioNMessage, new CancellationTokenSource().Token);
    }

    public async Task<DeviceConfigurationMessage> GetDeviceConfigurationMessage(Guid deviceId)
    {
        var deviceConfiguratioNMessage = new DeviceConfigurationMessage()
        {
            Bins = await _binRepository.GetBins(deviceId),
            KeyFobTags = await _keyFobTagRepository.GetKeyFobTags(deviceId)
        };

        return deviceConfiguratioNMessage;
    }
}


