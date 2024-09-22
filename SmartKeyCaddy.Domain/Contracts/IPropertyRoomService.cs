using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Contracts;
public interface IPropertyRoomService
{
    Task<List<PropertyRoom>> GetPropertyRooms(Guid propertyId);
}
