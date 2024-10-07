namespace SmartKeyCaddy.Models;

public class Location
{
    public Guid LocationId { get; set; }
    public Guid ChainId { get; set; }
    public Guid CountryId { get; set; }
    public Guid ParentRegionId { get; set; }
    public string LocationCode { get; set; }
    public string LocationName { get; set; }
    public bool IsActive {  get; set; }
}