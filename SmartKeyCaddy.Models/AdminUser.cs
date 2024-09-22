namespace SmartKeyCaddy.Models;

public class AdminUser
{
    public Guid AdminUserId { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public DateTime LastUpdated { get; set; }
    public List<Property> Properties { get; set; }
    public List<string> Roles { get; set; } 
    public List<string> Menus { get; set; }
}