﻿namespace SmartKeyCaddy.Models;

public class UserDetails
{
    public List<Property> AllowedProperties { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
    public List<string> Menus { get; set; } = new List<string>();
    public Guid ChainId { get; set; }
}