using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OtpNet;
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

    public string GenerateOtp()
    {
        string base32Secret = "JBSWY3DPEHPK3PXP"; // Replace with a unique Base32 secret

        int dailyTimeStep = 7200;

        // Convert the Base32 secret key to a byte array
        byte[] secretKey = Base32Encoding.ToBytes(base32Secret);

        // Create a TOTP instance with a 30-second time step (default for Google Authenticator)
        var totp = new Totp(secretKey, step: dailyTimeStep);

        // Generate a TOTP code for the current time
        string otpCode = totp.ComputeTotp();
        Console.WriteLine($"Generated OTP Code: {otpCode}");

        return otpCode;

    }
}