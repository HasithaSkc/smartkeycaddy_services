using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost]
    public async Task DisableBin(Guid deviceId, Guid binId)
    {
        await _adminService.DisableBin(deviceId, binId);
    }

    [HttpGet]
    [Route("Otp")]
    public IActionResult GenerateOtp()
    {
        return Ok(_adminService.GenerateOtp());
    }
}