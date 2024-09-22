using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Repository;

public interface IUserRepository
{
    Task<AdminUser> GetUser(string userName, string password);
    Task<ResourceUser> GetResourceUser(string userName, string password);
    Task<List<Property>> GetAdminUserProperties(Guid userId);
}
