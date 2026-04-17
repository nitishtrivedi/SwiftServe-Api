using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftServe_API.Services;

namespace SwiftServe_API.Controllers
{
    [ApiController]
    [Route("api/delivery")]
    [Authorize(Roles = "DeliveryPartner")]
    public class DeliveryController : ControllerBase
    {
        private readonly DeliveryService _service;

        public DeliveryController(DeliveryService service)
        {
            _service = service;
        }

        [HttpPost("location/{orderId}")]
        public async Task<IActionResult> UpdateLocation(int orderId, UpdateLocationDto dto)
        {
            await _service.UpdateLocation(dto, orderId);
            return Ok();
        }

        [HttpGet("orders")]
        public IActionResult GetOrders()
        {
            return Ok(_service.GetMyDeliveries());
        }

        [HttpPut("status/{orderId}")]
        public async Task<IActionResult> UpdateStatus(int orderId, [FromQuery] string status)
        {
            await _service.UpdateOrderStatus(orderId, status);
            return Ok();
        }
    }
}
