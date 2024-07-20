namespace SmartKeyCaddy.Models;

public class KeyAllocationResponse
{ 
    public Guid DeviceId { get; set; }
    public string DeviceName { get; set; }
    public List<KeyAllocationResponseItem> KeyAllocation { get; set; }
}

public class KeyAllocationResponseItem : KeyAllocationItem
{
    public string Status { get; set; } = string.Empty;
    public bool IsSuccessful { get; set; }
}