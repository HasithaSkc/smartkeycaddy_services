using SmartKeyCaddy.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartKeyCaddy.Domain.Contracts;

namespace SmartKeyCaddy.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("me")]
        public async Task<IActionResult> GetMe()
        {
            var adminUserIdStr = SecurityExtensions.GetSubjectId(User);

            if (!Guid.TryParse(adminUserIdStr, out Guid adminUserId))
                return BadRequest("User not found");

            return Ok(await _userService.GetMe(adminUserId));
        }
    }
}
