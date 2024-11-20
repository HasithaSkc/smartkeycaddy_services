namespace SmartKeyCaddy.Models;

public class Bin
{
    public Guid BinId { get; set; }
    public Guid DeviceId { get; set; }
    public int BinNumber { get; set; }
    public string BinAddress { get; set; }
    public Guid ChainId { get; set; }
    public Guid PropertyId { get; set; }
    public string Status { get; set; }
    public bool InUse { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime? LastUpdatedDateTime {  get; set; }
    public string CurrentKey { get; set; }
}