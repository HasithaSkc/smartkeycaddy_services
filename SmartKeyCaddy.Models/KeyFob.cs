using Newtonsoft.Json;

namespace SmartKeyCaddy.Models;

public class KeyFobTag
{
    public Guid KeyFobTagId { get; set; }
    public Guid ChaninId { get; set; }
    public Guid PropertyId { get; set; }
    public Guid PropertyRoomId { get; set; }

    [JsonProperty("KeyFobTag")]
    public string KeyFobTagCode { get; set; }
    public string RoomNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime CretedDateTime { get; set; }
    public DateTime LastUpdatedDateTime { get; set; }
}