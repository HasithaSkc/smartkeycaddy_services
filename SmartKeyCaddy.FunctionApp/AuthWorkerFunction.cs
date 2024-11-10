using Microsoft.Extensions.Logging;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Azure.Functions.Worker;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.FunctionApp;

public class AuthWorkerFunction
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthWorkerFunction> _logger;

    public AuthWorkerFunction(ILogger<AuthWorkerFunction> logger,
    IUserService userService, 
    IConfiguration configuration)
    {
        _logger = logger;
        _userService = userService;
        _configuration = configuration;
    }

    [Function("Auth")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/token")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var authRequest = JsonConvert.DeserializeObject<AuthRequest>(requestBody);

        if (!string.IsNullOrEmpty(authRequest.UserId) && !string.IsNullOrEmpty(authRequest.Password))
        {
            var user = await _userService.GetResourceUser(authRequest.UserId, authRequest.Password);

            if (user != null)
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, authRequest.UserId),
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var apiTokenResponse = new ApiTokenResponse()
                {
                    Token = tokenHandler.WriteToken(token),
                    TokenExpiry = tokenDescriptor.Expires.Value
                };

                return new OkObjectResult(apiTokenResponse);
            }
            else
            {
                return new BadRequestObjectResult("Invalid credentials");
            }
        }
        else
        {
            return new BadRequestObjectResult("Invalid credentials");
        }
    }
}
