using SmartKeyCaddy.Common;

namespace SmartKeyCaddy.Models;

public class KeyTransaction
{
    public Guid KeyTransactionId { get; set; }
    public Guid? KeyAllocationId { get; set; }
    public Guid ChainId { get; set; }
    public Guid PropertyId { get; set; }
    public Guid DeviceId { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public Guid? BinId { get; set; }
    public string KeyTransactionType { get; set; }
    public bool IsMessageSent { get; set; }
}