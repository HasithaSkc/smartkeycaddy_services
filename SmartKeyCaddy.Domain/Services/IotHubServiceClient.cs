using HotelCheckIn.Models.Configurations;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Options;
using SmartKeyCaddy.Domain.Contracts;

public class IotHubServiceClient : IIotHubServiceClient
{
    private readonly ServiceClient _serviceClient;
    private readonly IotHubSettings _iotHubSettings;

    public IotHubServiceClient(IOptions<IotHubSettings> iotHubSettings)
    {
        _iotHubSettings = iotHubSettings.Value;
        _serviceClient = ServiceClient.CreateFromConnectionString(_iotHubSettings.ConnectionString);
    }

    public async Task CloseConnection()
    {
        await _serviceClient.CloseAsync();
    }

    public async Task<CloudToDeviceMethodResult> SendMessageToDeviceAsync(string deviceId, CloudToDeviceMethod cloudToDeviceMethod)
    {
        return await _serviceClient.InvokeDeviceMethodAsync(deviceId, cloudToDeviceMethod);
    }

    public async Task<bool> IsDeviceOnlineAsync(string deviceId)
    {
        try
        {
            var registryManager = RegistryManager.CreateFromConnectionString(_iotHubSettings.ConnectionString);
            var device = await registryManager.GetDeviceAsync(deviceId);

            if (device == null)
               return false;

            var twin = await registryManager.GetTwinAsync(deviceId);
            return twin?.ConnectionState?.ToString() ==  "Connected";
        }
        catch
        {
            return false;
        }
    }
}