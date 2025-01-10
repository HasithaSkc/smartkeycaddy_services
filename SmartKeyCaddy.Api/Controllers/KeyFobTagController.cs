using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartKeyCaddy.Domain.Contracts;

namespace SmartKeyCaddy.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class KeyFobTagController : ControllerBase
{
    private readonly IKeyFobTagService _keyFobTagService;

    public KeyFobTagController(IKeyFobTagService keyFobTagService)
    {
        _keyFobTagService = keyFobTagService;
    }

    [HttpGet]
    [Route("{propertyId:guid}")]
    public async Task<IActionResult> GetKeyFobTags(Guid propertyId)
    {
        return Ok(await _keyFobTagService.GetKeyFobTags(propertyId));
    }
}