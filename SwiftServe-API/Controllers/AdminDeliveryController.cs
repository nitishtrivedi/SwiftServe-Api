using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftServe_API.Services;

namespace SwiftServe_API.Controllers
{
    [ApiController]
    [Route("api/admin/delivery")]
    [Authorize(Roles = "Admin")]
    public class AdminDeliveryController : ControllerBase
    {
        private readonly DeliveryService _service;

        public AdminDeliveryController(DeliveryService service)
        {
            _service = service;
        }

        [HttpPost("assign")]
        public async Task<IActionResult> Assign(AssignDeliveryDto dto)
        {
            await _service.AssignDelivery(dto);
            return Ok("Assigned");
        }
    }
}
