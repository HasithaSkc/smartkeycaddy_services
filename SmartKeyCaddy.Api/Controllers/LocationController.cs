using Microsoft.AspNetCore.Mvc;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LocationController : BaseController
{
    private readonly ILocationService _locationService;

    public LocationController(IUserService userService, 
        ILocationService locationService) : base(userService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetLocations(Guid chainId)
    {
        return Ok(await _locationService.GetLocations(chainId));
    }

    [HttpGet]
    [Route("{locationId:guid}")]
    public async Task<IActionResult> GetLocation(Guid locationId)
    {
        return Ok(await _locationService.GetLocation(locationId));
    }

    [HttpPost]
    [Route("")]
    public async Task AddLocation([FromBody] Location location)
    {
        var userDetails = await GetUser();
        await _locationService.AddLocation(location, userDetails.ChainId);
    }

    [HttpPost]
    [Route("{parentId:guid}")]
    public async Task AddSubLocation([FromRoute] Guid parentId, [FromBody] Location location)
    {
        var userDetails = await GetUser();
        await _locationService.AddLocation(parentId, userDetails.ChainId);
    }

    [HttpPut]
    [Route("")]
    public async Task UpdateLocation([FromBody] Location location)
    {
        var userDetails = await GetUser();
        await _locationService.UpdateLocation(location, userDetails.ChainId);
    }

    [HttpDelete]
    [Route("{locationId:guid}")]
    public async Task DeleteLocation(Guid locationId)
    {
        await _locationService.DeleteLocation(locationId);
    }
}