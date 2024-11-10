namespace SmartKeyCaddy.Models;

public class BinOpenResponse
{
    public Guid BinId { get; set; }
    public bool HasKeyAllocation { get; set; }
}