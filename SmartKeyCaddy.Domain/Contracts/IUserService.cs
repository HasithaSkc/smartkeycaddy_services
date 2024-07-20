using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Contracts;

public interface IUserService
{
    Task<UserInfo> GetUser(string userId, string password);
    Task<UserInfo> GetResourceUser(string userId, string password);
}
