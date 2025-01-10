namespace SmartKeyCaddy.Models;

public class KeyAllocationRequest
{
    public Guid DeviceId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public List<KeyAllocationItem> KeyAllocation { get; set; }
}

public class KeyAllocationItem
{
    public Guid? KeyAllocationId { get; set; }
    public string KeyName { get; set; }
    public string RoomNumber { get; set; }
    public string KeyPinCode { get; set; }
    public string GuestWelcomeMessage { get; set; } = string.Empty;
    public string KeyPickupInstruction { get; set; } = string.Empty;
    public string CheckInDate { get; set; } = string.Empty;
    public string CheckOutDate { get; set; } = string.Empty;
}   