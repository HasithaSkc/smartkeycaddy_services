using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class KeyAllocationController : ControllerBase
    {
        private readonly IKeyAllocationService _keyAllocationService;

        public KeyAllocationController(IKeyAllocationService keyReservationService)
        {
            _keyAllocationService = keyReservationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateKey(KeyAllocationRequest createKeyRequest)
        {
            return Ok(await _keyAllocationService.CreateKeyAllocation(createKeyRequest));
        }

        [HttpGet]
        [Route("{deviceId:guid}")]
        public async Task<IActionResult> GetKeyAllocations([FromRoute] Guid deviceId)
        {
            return Ok(await _keyAllocationService.GetKeyAllocations(deviceId));
        }

        [HttpGet]
        [Route("forcebinopen/{deviceId:guid}/{binId:guid}")]
        public async Task<IActionResult> ForceBinOpen([FromRoute] Guid deviceId, [FromRoute] Guid binId)
        {
            await _keyAllocationService.ForceBinOpen(deviceId, binId);
            return Accepted();
        }
    }
}