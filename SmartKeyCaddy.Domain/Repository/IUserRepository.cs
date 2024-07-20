using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Repository
{
    public interface IUserRepository
    {
        Task<UserInfo> GetUser(string userName, string password);
        Task<UserInfo> GetResourceUser(string userName, string password);
    }
}
