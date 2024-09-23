using SmartKeyCaddy.Common;
using SmartKeyCaddy.Models;
using Microsoft.AspNetCore.Mvc;
using SmartKeyCaddy.Domain.Contracts;

namespace SmartKeyCaddy.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        public AuthController(IUserService userService,
            ITokenService tokenService,
            IConfiguration configuration)
        {
            _userService = userService;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        /// <summary>
        /// UserId/Password authentication from Admin Website.
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> GetToken([FromBody] LoginRequest loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.UserName) || string.IsNullOrEmpty(loginRequest.Password))
                return Unauthorized();

            var user = await _userService.GetAdminUser(loginRequest.UserName, loginRequest.Password);

            if (user == null)
                return Unauthorized();

            return Ok(_tokenService.GetToken(user.AdminUserId));
        }

        /// <summary>
        /// Kiosk authentication
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        [HttpPost]
        [Route("resourceToken")]
        public async Task<IActionResult> GetResourceToken([FromBody] LoginRequest loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.UserName) || string.IsNullOrEmpty(loginRequest.Password))
                return Unauthorized();

            var user = await _userService.GetResourceUser(loginRequest.UserName, loginRequest.Password);

            if (user == null)
                return Unauthorized();

            return Ok(_tokenService.GetToken(user.ResourceUserId));
        }
    }
}
