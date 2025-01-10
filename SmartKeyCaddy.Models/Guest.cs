namespace SmartKeyCaddy.Models;

public class Guest
{

    public Guid GuestId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Mobile { get; set; }
    public int PmsGuestId { get; set; }
    public int AccountId { get; set; }

    public Guid ChainId { get; set; }
    public string Notes { get; set; }  
}