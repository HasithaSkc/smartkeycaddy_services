using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartKeyCaddy.Domain.Contracts;

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
    [Route("offlinepincode/{deviceId:guid}/{roomNumber}")]
    public async Task<IActionResult> GenerateOfflinePinCode(Guid deviceId, string roomNumber)
    {
        return Ok(await _adminService.GenerateOfflinePinCode(deviceId, roomNumber));
    }
}