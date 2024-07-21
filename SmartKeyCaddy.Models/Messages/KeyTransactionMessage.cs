namespace SmartKeyCaddy.Models.Messages;

public class KeyTransactionMessage: BaseMessage
{
    public string KeyName { get; set; }
    public Guid? KeyAllocationId { get; set; }
    public Guid? KeyFobTagId { get; set; }
    public string KeyFobTag { get; set; }
    public string KeyPinCode { get; set; }
    public Guid? BinId { get; set; }
    public int? BinNumber { get; set; }
    public string Status { get; set; }
    public bool IsSuccessful { get; set; }
    public DateTime CreatedDateTime { get; set; }
}