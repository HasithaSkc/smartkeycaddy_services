namespace SmartKeyCaddy.Models.Messages;

public class KeyTransactionMessage
{ 
    public string MessageType { get; set; }
    public List<KeyTransactionItem> KeyTransactions { get; set; }  
}

public class KeyTransactionItem : BaseMessage
{
    public Guid KeyTransactionId { get; set; }
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