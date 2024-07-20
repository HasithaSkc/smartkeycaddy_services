namespace SmartKeyCaddy.Models.Messages;

public class KeyTransactionMessage
{
    public Guid KeyTransactionId { get; set; }
    public Guid KeyAllocationId { get; set; }
    public Guid DeviceId { get; set; }
    public Guid DeviceName { get; set; }
    public string KeyName { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public Guid? BinId { get; set; }
    public int? BinNumber { get; set; }
    public string Status { get; set; }
    public bool IsSuccessful { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime PublishedDateTime { get; set; }
    public string keyTransactionType { get; set; }
}