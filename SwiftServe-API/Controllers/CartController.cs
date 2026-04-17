using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftServe_API.DTOs;
using SwiftServe_API.Services;

namespace SwiftServe_API.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Authorize(Roles = "Customer")]
    public class CartController : ControllerBase
    {
        private readonly CartService _service;

        public CartController(CartService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddToCartDto dto)
        {
            await _service.AddToCart(dto);
            return Ok("Added to cart");
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _service.GetCart());
        }
    }
}
