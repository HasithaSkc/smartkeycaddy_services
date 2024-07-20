namespace SmartKeyCaddy.Models;

public class KeyAllocation
{
    public Guid? KeyAllocationId { get; set; }
    public Guid DeviceId { get; set; }
    public string KeyName { get; set; }
    public Guid? KeyFobTagId { get; set; }
    public string KeyFobTag { get; set; }
    public string KeyPinCode { get; set; }
    public string GuestWelcomeMessage { get; set; }
    public string KeyPickupInstruction { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public Guid? BinId { get; set; }
    public int? BinNumber { get; set; }
    public string Status { get; set; }
    public bool IsSuccessful { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public Guid ChainId { get; set; }
    public Guid PropertyId { get; set; }
}