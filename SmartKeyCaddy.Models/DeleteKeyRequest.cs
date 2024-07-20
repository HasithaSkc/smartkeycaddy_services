namespace SmartKeyCaddy.Models;

public class DeleteKeyRequest
{
    public Guid DeviceId { get; set; }
    public Guid KeyId { get; set; }
}