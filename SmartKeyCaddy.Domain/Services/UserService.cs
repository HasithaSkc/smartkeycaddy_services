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

    public UserService(IUserRepository userRepository,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<AdminUser> GetAdminUser(string userName, string password)
    {
        var hashedPassword = PasswordHash.HashPasword(password);
        return await _userRepository.GetUser(userName, hashedPassword);
    }

    public async Task<ResourceUser> GetResourceUser(string userId, string password)
    {
        var hashedPassword = PasswordHash.HashPasword(password);
        return await _userRepository.GetResourceUser(userId, hashedPassword);
    }

    public async Task<UserDetails> GetMe(Guid adminUserId)
    {
        var userDetails = new UserDetails()
        {
            AllowedProperties = await _userRepository.GetAdminUserProperties(adminUserId),
        };

        return userDetails;
    }

    public Task UpdateUser(UserDetails userDetails)
    {
        throw new NotImplementedException();
    }
}