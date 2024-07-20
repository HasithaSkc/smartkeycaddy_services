namespace SmartKeyCaddy.Models;

public class PropertyRoom
{
    public Guid KeyFobTagId { get; set; }
    public Guid PropertyRoomId { get; set; }
    public Guid PropertyId { get; set; }
    public string KeyFobTag { get; set; }
    public string RoomNumber { get; set; }
}