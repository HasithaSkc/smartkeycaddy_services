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
        private readonly IKeyAllocationService _keyReservationService;

        public KeyAllocationController(IKeyAllocationService keyReservationService)
        {
            _keyReservationService = keyReservationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateKey(KeyAllocationRequest createKeyRequest)
        {
            return Ok(await _keyReservationService.CreateKeyAllocation(createKeyRequest));
        }

        //[HttpGet]
        //[Route("{Guid: deviceId}")]
        //public async Task<IActionResult> GetKeyAllocations([FromRoute]Guid deviceId)
        //{
        //    return Ok(await _keyReservationService.GetKeyAllocations(deviceId));
        //}

        //[HttpGet]
        //[Route("{Guid: keyAllocationId}")]
        //public async Task<IActionResult> GetKeyAllocation([FromRoute] Guid keyAllocationId)
        //{
        //    return Ok(await _keyReservationService.GetKeyAllocation(keyAllocationId));
        //}
    }
}