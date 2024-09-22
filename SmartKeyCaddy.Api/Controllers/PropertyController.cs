using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyRoomService _propertyRoomService;

        public PropertyController(IPropertyRoomService propertyRoomService)
        {
            _propertyRoomService = propertyRoomService;
        }

        [HttpGet]
        [Route("{propertyId}")]
        public async Task<IActionResult> GetPropertyRooms(Guid propertyId)
        {
            return Ok(await _propertyRoomService.GetPropertyRooms(propertyId));
        }
    }
}