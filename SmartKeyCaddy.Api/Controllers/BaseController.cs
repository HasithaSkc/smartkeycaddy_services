using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Api.Controllers;

[Authorize]
public class BaseController : ControllerBase
{
    private readonly IUserService _userService;

    public BaseController()
    {
    }

    public BaseController(IUserService userService)
    {
        _userService = userService;
    }

    protected async Task<UserDetails> GetUser()
    {
        if (!Guid.TryParse(SecurityExtensions.GetSubjectId(User), out Guid adminUserId))
            throw new BadHttpRequestException("Invalid user");

        return await _userService.GetMe(adminUserId);
    }
}