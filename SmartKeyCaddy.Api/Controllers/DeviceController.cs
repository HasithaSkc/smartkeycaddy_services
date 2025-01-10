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
    [Route("list/{propertyId:guid}")]
    public async Task<IActionResult> GetDevices(Guid propertyId)
    {
        return Ok(await _deviceService.GetDevices(propertyId));
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

    [HttpGet]
    [Route("{deviceId:guid}/bins")]
    public async Task<IActionResult> GetDeviceBinDetails(Guid deviceId)
    {
        return Ok(await _deviceService.GetDeviceBinDetails(deviceId));
    }

    [HttpGet]
    [Route("status/{deviceId:guid}")]
    public async Task<IActionResult> GetDeviceOnlineStatus(Guid deviceId)
    {
        return Ok(await _deviceService.GetDeviceOnlineStatus(deviceId));
    }

    [HttpGet]
    [Route("log/{deviceId:guid}")]
    public async Task<IActionResult> GetDeviceLog(Guid deviceId)
    {
        var fileContent = await _deviceService.GetDeviceLog(deviceId);
        return File(fileContent.Item1, "text/plain", $"{fileContent.Item2}-log.txt");
    }
}