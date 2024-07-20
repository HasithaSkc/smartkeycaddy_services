using Microsoft.Extensions.Logging;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Services;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserInfo> GetUser(string userId, string password)
    {
        var hashedPassword = PasswordHash.HashPasword(password);
        return await _userRepository.GetUser(userId, hashedPassword);
    }

    public async Task<UserInfo> GetResourceUser(string userId, string password)
    {
        var hashedPassword = PasswordHash.HashPasword(password);
        return await _userRepository.GetResourceUser(userId, hashedPassword);
    }
}
