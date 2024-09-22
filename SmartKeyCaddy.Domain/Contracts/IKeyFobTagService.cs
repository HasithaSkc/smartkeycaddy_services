
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Contracts;

public interface IKeyFobTagService
{
    Task<List<KeyFobTag>> GetKeyFobTags(Guid propertyId);
    Task<List<KeyFobTag>> GetPropertyRoomKeyFobTags(Guid propertyId);
}
