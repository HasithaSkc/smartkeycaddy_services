using SmartKeyCaddy.Models.Messages;

namespace SmartKeyCaddy.Models;

public class DeviceKeyAllocationRequest: BaseMessage
{
    public List<DeviceKeyAllocationItem> KeyAllocation { get; set; }
}

public class DeviceKeyAllocationItem
{
    public Guid? KeyAllocationId { get; set; }
    public Guid? KeyFobTagId { get; set; }
    public string KeyName { get; set; }
    public string KeyPinCode { get; set; }
    public string GuestWelcomeMessage { get; set; } = string.Empty;
    public string KeyPickupInstruction { get; set; } = string.Empty;
    public string CheckInDate { get; set; } = string.Empty;
    public string CheckOutDate { get; set; } = string.Empty;
}   