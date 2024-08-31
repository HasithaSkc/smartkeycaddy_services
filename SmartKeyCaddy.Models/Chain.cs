namespace SmartKeyCaddy.Models;
public class Chain
{
    public Guid ChainId { get; set; }
    public string ChainName { get; set;}
    public bool IsActive { get; set; }
    public string LogoUrl { get; set; }
    public string CroNumber { get; set; }
    public string ChainCode { get; set; }
}