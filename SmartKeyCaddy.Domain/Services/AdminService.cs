using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OtpNet;
using SmartKeyCaddy.Common.JsonHelper;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models.Exceptions;
using SmartKeyCaddy.Models.Messages;
using System.Text;

namespace SmartKeyCaddy.Domain.Services;

public partial class AdminService : IAdminService
{
    private readonly ILogger<KeyAllocationService> _logger;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IBinRepository _binRepository;
    private readonly IKeyFobTagRepository _keyFobTagRepository;
    private readonly IIotHubServiceClient _iotHubServiceClient;
    private readonly IPropertyRepository _propertyRepository;

    public AdminService(ILogger<KeyAllocationService> logger,
        IDeviceRepository deviceRepository,
        IBinRepository binRepository,
        IKeyFobTagRepository keyFobTagRepository,
        IIotHubServiceClient iotHubServiceClient,
        IPropertyRepository propertyRepository)
    {
        _logger = logger;
        _deviceRepository = deviceRepository;
        _binRepository = binRepository;
        _keyFobTagRepository = keyFobTagRepository;
        _iotHubServiceClient = iotHubServiceClient;
        _propertyRepository = propertyRepository;
    }

    public async Task RegisterDevice(DeviceRegisterMessage registerDeviceMessage)
    {
        var device = await _deviceRepository.GetDevice(registerDeviceMessage.DeviceId, registerDeviceMessage.DeviceName);
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

    public async Task<string> GenerateOfflinePinCode(Guid deviceId, string roomNumber)
    {
        var device = await _deviceRepository.GetDevice(deviceId);

        if (device == null)
            throw new NotFoundException("Device not found");

        roomNumber = roomNumber.Replace(" ", "");
        var secret = $"{device.DeviceId}{roomNumber}";

        // Encode the GUID bytes to Base32 using OtpNet
        var base32Secret = Base32Encoding.ToString(Encoding.UTF8.GetBytes(secret));

        // Decode the Base32 encoded secret key
        var key = Base32Encoding.ToBytes(base32Secret);

        // Create a TOTP valid for 1 day
        var totp = new Totp(key, step: 60 * 60 * 24 * 2);

        // Generate the current TOTP
        return totp.ComputeTotp();
    }
}