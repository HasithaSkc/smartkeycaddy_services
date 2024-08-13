namespace SmartKeyCaddy.Models.Messages;

public class BaseMessage
{
    public string MessageType { get; set; }
    public Guid DeviceId { get; set; }
    public string DeviceName { get; set; }
    public DateTime EnqueuedDateTime { get; set; }
}