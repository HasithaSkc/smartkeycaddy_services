using Microsoft.Azure.Devices;

namespace SmartKeyCaddy.Domain.Contracts;

public interface IIotHubServiceClient
{
    Task<CloudToDeviceMethodResult> SendDirectMessageToDevice(string deviceName, CloudToDeviceMethod cloudToDeviceMethod);
    Task SendIndirectMessageToDevice(string deviceName, string message);
    Task<bool> IsDeviceOnline(string deviceName);
    Task CloseConnection();
}
