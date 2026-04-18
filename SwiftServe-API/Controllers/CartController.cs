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

        // ✅ ADD ITEM
        [HttpPost("add")]
        public async Task<IActionResult> Add(AddToCartDto dto)
        {
            await _service.AddToCart(dto);
            return Ok("Item added to cart");
        }

        // ✅ GET CART
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var cart = await _service.GetCart();
            return Ok(cart);
        }

        // ✅ UPDATE QUANTITY
        [HttpPut("update")]
        public async Task<IActionResult> Update(UpdateCartItemDto dto)
        {
            await _service.UpdateCartItem(dto);
            return Ok("Cart updated");
        }

        // ✅ REMOVE ITEM
        [HttpDelete("remove/{menuItemId}")]
        public async Task<IActionResult> Remove(int menuItemId)
        {
            await _service.RemoveItem(menuItemId);
            return Ok("Item removed");
        }

        // ✅ CLEAR CART
        [HttpDelete("clear")]
        public async Task<IActionResult> Clear()
        {
            await _service.ClearUserCart();
            return Ok("Cart cleared");
        }
    }
}
