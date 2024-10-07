using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class DeviceController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DeviceController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet]
    [Route("{Guid:deviceId}")]
    public async Task<IActionResult> GetDevice(Guid deviceId)
    {
        return Ok(await _deviceService.GetDevice(deviceId));
    }

    [HttpGet]
    [Route("{Guid:propertyId}")]
    public async Task<IActionResult> GetDevices(Guid propertyId)
    {
        return Ok(await _deviceService.GetDevicesForProperty(propertyId));
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> AddDevice([FromBody] Device device)
    {
        return Ok(await _deviceService.AddDevice(device));
    }

    [HttpPut]
    [Route("")]
    public async Task<IActionResult> UpdateDevice([FromBody] Device device)
    {
        return Ok(await _deviceService.UpdateDevice(device));
    }

    [HttpDelete]
    [Route("{Guid:deviceId}")]
    public async Task<IActionResult> DeleteDevice(Guid deviceId)
    {
        return Ok(await _deviceService.DeleteDevice(deviceId));
    }
}