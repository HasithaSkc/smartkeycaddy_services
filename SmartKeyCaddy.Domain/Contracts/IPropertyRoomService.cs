using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Contracts;
public interface IPropertyRoomService
{
    Task<List<PropertyRoom>> GetPropertyRooms(Guid propertyId);
    Task<List<PropertyRoom>> GetPropertyRoomsKeyFobTags(Guid propertyId);
    Task InsertOrUpdatePropertyRoomsKeyFobTag(PropertyRoomKeyFobtag propertyRoomkeyFobTag, Guid propertyId);
}
