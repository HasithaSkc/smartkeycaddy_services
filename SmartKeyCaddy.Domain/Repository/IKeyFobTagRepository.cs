using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Repository
{
    public interface IKeyFobTagRepository
    {
        Task<List<KeyFobTag>> GetKeyFobTags(Guid deviceId);
    }
}
