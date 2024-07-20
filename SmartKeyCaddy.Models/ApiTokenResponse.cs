using System.Text.Json.Serialization;

namespace SmartKeyCaddy.Models;

public class ApiTokenResponse
{
    public string Token { get; set; }

    public DateTime TokenExpiry { get; set; }
}
