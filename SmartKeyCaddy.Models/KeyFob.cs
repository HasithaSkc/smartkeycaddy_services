namespace SmartKeyCaddy.Models;

public class KeyFobTag
{
    public Guid KeyFobTagId { get; set; }
    public Guid ChaninId { get; set; }
    public Guid PropertyId { get; set; }
    public Guid PropertyRoomId { get; set; }
    public string keyFobTag { get; set; }
    public bool IsActive { get; set; }
    public DateTime CretedDateTime { get; set; }
    public DateTime LastUpdatedDateTime { get; set; }
}