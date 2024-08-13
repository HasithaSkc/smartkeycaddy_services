namespace SmartKeyCaddy.Models.Messages;

public class RegisterDeviceMessage : BaseMessage
{
    public DateTime RegisteredDateTime { get; set; }
}