using SmartKeyCaddy.Models;
using System.Threading.Tasks;

namespace SmartKeyCaddy.Domain.Contracts;
public interface IPropertyRoomService
{
    Task<List<PropertyRoom>> GetPropertyRooms(Guid propertyId);
    Task<List<PropertyRoom>> GetPropertyRoomsKeyFobTags(Guid propertyId);
    Task InsertPropertyRoomsKeyFobTags(List<PropertyRoomKeyFobtag> propertyRoomKeyFobTags, Guid propertyId);
}
