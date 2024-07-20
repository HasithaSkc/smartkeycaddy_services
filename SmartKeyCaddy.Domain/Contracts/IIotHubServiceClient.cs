using Microsoft.Azure.Devices;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Contracts;

public interface IIotHubServiceClient
{
    Task<CloudToDeviceMethodResult> SendMessageToDeviceAsync(string deviceId, CloudToDeviceMethod cloudToDeviceMethod);
    Task<bool> IsDeviceOnlineAsync(string deviceName);
    Task CloseConnection();
}
