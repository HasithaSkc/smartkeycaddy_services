namespace SmartKeyCaddy.Models;
public class Property 
{
    public Guid PropertyId { get; set; }
    public Guid ChainId { get; set; }
    public string PmsPropertyId { get; set; }
    public string PropertyName { get; set; }
    public string PropertyCode { get; set; }
    public string PropertyShortCode { get; set; }
    public string AddressLine1 { get; set; }
    public string City { get; set; }
    public string StateCode { get; set; }
    public string PostCode { get; set; }
    public Guid CountryId { get; set; }
    public string CountryCode { get; set; }
    public Guid RegionId { get; set; }
    public string RegionCode { get; set; }
    public string Abn { get; set; }
    public bool IsActive { get; set; }
    public string TimeZone { get; set; }
    public Chain Chain { get; set; }

    //Property settings
    public string BackgroundImageUrl { get; set; }
    public string WelcomeMessage { get; set; }
    public string PropertyLogo { get; set; }
}
