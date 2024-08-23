namespace SmartKeyCaddy.Models;

public class Bin
{
    public Guid BinId { get; set; }
    public int BinNumber { get; set; }
    public string BinAddress { get; set; }
    public Guid ChaninId { get; set; }
    public string PropertyId { get; set; }
    public string Status { get; set; }
    public bool InUse { get; set; }
    public DateTime CretedDateTime { get; set; }
    public DateTime LastUpdatedDateTime {  get; set; }
}