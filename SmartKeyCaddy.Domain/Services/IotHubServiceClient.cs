using HotelCheckIn.Models.Configurations;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Models.Configurations;
using System.Text;

namespace SmartKeyCaddy.Domain.Services;
public class IotHubServiceClient : IIotHubServiceClient
{
    private readonly ServiceClient _serviceClient;
    private readonly IotHubSettings _iotHubSettings;
    private readonly ILogger<IotHubServiceClient> _logger;
    public IotHubServiceClient(IOptions<IotHubSettings> iotHubSettings,
        ILogger<IotHubServiceClient> logger)
    {
        _iotHubSettings = iotHubSettings.Value;
        _logger = logger;
        _serviceClient = ServiceClient.CreateFromConnectionString(_iotHubSettings.ConnectionString);
    }

    public async Task CloseConnection()
    {
        await _serviceClient.CloseAsync();
    }

    public async Task<CloudToDeviceMethodResult> SendDirectMessageToDevice(string deviceName, CloudToDeviceMethod cloudToDeviceMethod)
    {
        return await _serviceClient.InvokeDeviceMethodAsync(deviceName, cloudToDeviceMethod);
    }

    public async Task<bool> IsDeviceOnline(string deviceName)
    {
        try
        {
            var registryManager = RegistryManager.CreateFromConnectionString(_iotHubSettings.ConnectionString);
            var device = await registryManager.GetDeviceAsync(deviceName);

            if (device == null)
               return false;

            var twin = await registryManager.GetTwinAsync(deviceName);
            return twin?.ConnectionState?.ToString() ==  "Connected";
        }
        catch
        {
            return false;
        }
    }

    public async Task SendIndirectMessageToDevice(string deviceName, string message)
    {
        var messageBody = new Message(Encoding.ASCII.GetBytes(message));
        messageBody.ExpiryTimeUtc = DateTime.UtcNow.AddDays(1);
        await _serviceClient.SendAsync(deviceName, messageBody);
        _logger.LogInformation($"Successfully sent the messge:{message}");
    }
}