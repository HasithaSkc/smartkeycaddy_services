using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Repository;

public interface IPropertyRoomRepository
{
    Task<List<PropertyRoom>> GetPropertyRooms(Guid propertyId);
    Task<List<PropertyRoom>> GetPropertyRoomKeyFobTags(Guid propertyId);
    Task InsertPropertyRoomKeyFobTag(PropertyRoomKeyFobtag propertyRoomkeyFobTag, Guid propertyId, Guid chainId);
    Task DeletePropertyRoomKeyFobTags(Guid propertyId);
}
