namespace SmartKeyCaddy.Models;

public class DeviceKeyAllocationResponse
{ 
    public Guid DeviceId { get; set; }
    public string DeviceName { get; set; }
    public List<DeviceKeyAllocationResponseItem> KeyAllocation { get; set; }
}

public class DeviceKeyAllocationResponseItem : KeyAllocationItem
{
    public string Status { get; set; } = string.Empty;
    public bool IsSuccessful { get; set; }
}