using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftServe_API.DTOs;
using SwiftServe_API.Services;

namespace SwiftServe_API.Controllers
{
    [ApiController]
    [Route("api/payment")]
    [Authorize(Roles = "Customer")]
    public class PaymentController : ControllerBase
    {
        private readonly RazorpayService _service;

        public PaymentController(RazorpayService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public IActionResult Create(CreatePaymentDto dto)
        {
            return Ok(_service.CreateOrder(dto.OrderId));
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify(VerifyPaymentDto dto)
        {
            await _service.VerifyPayment(dto);
            return Ok("Payment successful");
        }
    }
}
