namespace SmartKeyCaddy.Models;

public class Device
{
    public Guid DeviceId { get; set; }
    public string DeviceCode { get; set; }
    public string DeviceName { get; set; }
    public string SerialNumber { get; set; }
    public int BinCount { get; set; }
    public Guid PropertyId { get; set; }
    public bool IsActive {  get; set; }
    public bool IsMaster { get; set; }
    public Guid? MasterDeviceId { get; set; }
    public Guid ChainId { get; set; }
    public bool IsRegistered { get; set; }
}