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
    [Route("{deviceId:guid}")]
    public async Task<IActionResult> GetDevice(Guid deviceId)
    {
        return Ok(await _deviceService.GetDevice(deviceId));
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
    [Route("{deviceId:guid}")]
    public async Task<IActionResult> DeleteDevice(Guid deviceId)
    {
        return Ok(await _deviceService.DeleteDevice(deviceId));
    }

    [HttpDelete]
    [Route("bins/{deviceId:guid}")]
    public async Task<IActionResult> GetDeviceBinDetails(Guid deviceId)
    {
        return Ok(await _deviceService.DeleteDevice(deviceId));
    }
}