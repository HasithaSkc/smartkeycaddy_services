using Newtonsoft.Json;

namespace SmartKeyCaddy.Models;

public class KeyFobTag
{
    public Guid KeyFobTagId { get; set; }
    public Guid ChainId { get; set; }
    public Guid PropertyId { get; set; }

    [JsonProperty("KeyFobTag")]
    public string KeyFobTagCode { get; set; }
    public string KeyFobRfId { get; set; }
    public string RoomNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime? LastUpdatedDateTime { get; set; }
    public bool? IsAssigned { get; set; }
    public Guid? PropertyRoomId { get; set; }
}