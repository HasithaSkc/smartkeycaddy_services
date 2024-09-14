namespace SmartKeyCaddy.Models.Messages;

public class KeyTransactionMessage : BaseMessage
{ 
    public List<KeyTransaction> KeyTransactions { get; set; }  
}