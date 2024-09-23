namespace SmartKeyCaddy.Models;

public class PropertyRoom
{
    public Guid PropertyRoomId { get; set; }
    public Guid PropertyId { get; set; }
    public Guid ChainId { get; set; }
    public string RoomNumber { get; set; }
    public KeyFobTag KeyFobTag { get; set; }
}