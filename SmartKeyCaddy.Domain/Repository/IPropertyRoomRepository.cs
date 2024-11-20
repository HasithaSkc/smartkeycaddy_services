using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Repository;

public interface IPropertyRoomRepository
{
    Task<List<PropertyRoom>> GetPropertyRooms(Guid propertyId);
    Task<List<PropertyRoom>> GetPropertyRoomKeyFobTags(Guid propertyId);
    Task<PropertyRoom> GetPropertyRoomByKeyFobTag(Guid keyFobTagId, Guid propertyId);
    Task<PropertyRoomKeyFobtag> GetPropertyRoomKeyFobTag(Guid propertyId, Guid propertyRoomId);
    Task InsertPropertyRoomKeyFobTag(PropertyRoomKeyFobtag propertyRoomkeyFobTag, Guid propertyId, Guid chainId);
    Task UpdatePropertyRoomKeyFobTag(PropertyRoomKeyFobtag propertyRoomkeyFobTag, Guid propertyId);
    Task DeletePropertyRoomKeyFobTag(Guid propertyId, Guid propertyRoomId);
}
