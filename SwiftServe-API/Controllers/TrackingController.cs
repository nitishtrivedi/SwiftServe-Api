using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftServe_API.Services;

namespace SwiftServe_API.Controllers
{
    [ApiController]
    [Route("api/tracking")]
    [Authorize]
    public class TrackingController : ControllerBase
    {
        private readonly DeliveryService _service;

        public TrackingController(DeliveryService service)
        {
            _service = service;
        }

        [HttpGet("{deliveryPartnerId}")]
        public IActionResult Track(int deliveryPartnerId)
        {
            var location = _service.GetLatestLocation(deliveryPartnerId);
            return Ok(location);
        }
    }
}
