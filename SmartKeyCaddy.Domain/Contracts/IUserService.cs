using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Contracts;

public interface IUserService
{
    Task<AdminUser> GetAdminUser(string userId, string password);
    Task<ResourceUser> GetResourceUser(string userId, string password);
    Task<UserDetails> GetMe(Guid adminUserId);
    Task UpdateUser(UserDetails userDetails);
}
