namespace SmartKeyCaddy.Models.Messages;

public class DisableBinMessage : BaseMessage
{
    public Guid BinId { get; set; }
}