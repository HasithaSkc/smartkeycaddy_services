using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Repository;

public interface IPropertyRoomRepository
{
    Task<List<PropertyRoom>> GetPropertyRooms(Guid propertyId);
}
