using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class PropertyController : ControllerBase
{
    private readonly IPropertyRoomService _propertyRoomService;
    private readonly IPropertyService _propertyService;

    public PropertyController(IPropertyRoomService propertyRoomService, 
        IPropertyService propertyService)
    {
        _propertyRoomService = propertyRoomService;
        _propertyService = propertyService;
    }

    [HttpGet]
    [Route("room/{Guid: propertyId}")]
    public async Task<IActionResult> GetPropertyRooms(Guid propertyId)
    {
        return Ok(await _propertyRoomService.GetPropertyRooms(propertyId));
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetProperties()
    {
        return Ok(await _propertyService.GetProperties());
    }

    [HttpGet]
    [Route("{Guid: propertyId}")]
    public async Task<IActionResult> GetProperty(Guid propertyId)
    {
        return Ok(await _propertyService.GetProperty(propertyId));
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> AddProperty(Property property)
    {
        return Ok(await _propertyService.AddProperty(property));
    }

    [HttpPut]
    [Route("{Guid: propertyId}")]
    public async Task<IActionResult> UpdateProperty(Property property)
    {
        return Ok(await _propertyService.UpdateProperty(property));
    }

    [HttpDelete]
    [Route("{Guid: propertyId}")]
    public async Task<IActionResult> DeleteProperty(Guid propertyId)
    {
        return Ok(await _propertyService.DeleteProperty(propertyId));
    }
}