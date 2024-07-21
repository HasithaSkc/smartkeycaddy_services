namespace SmartKeyCaddy.Models.Messages;

public class ServiceBusMessage: BaseMessage
{
    public Guid MessageId { get; set; }
    public string MessageBody { get; set; }
    public bool IsProcessed { get; set; }
}