using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftServe_API.DTOs;
using SwiftServe_API.Services;

namespace SwiftServe_API.Controllers
{
    [ApiController]
    [Route("api/admin/menu")]
    [Authorize(Roles = "Admin")]
    public class MenuController : ControllerBase
    {
        private readonly MenuService _service;

        public MenuController(MenuService service)
        {
            _service = service;
        }

        // ✅ CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateMenuItemDto dto, IFormFile file)
        {
            await _service.CreateMenuItem(dto, file);
            return Ok("Menu item created");
        }

        // ✅ GET MENU (restaurant wise)
        [HttpGet("{restaurantId}")]
        [AllowAnonymous]
        public IActionResult GetMenu(int restaurantId)
        {
            return Ok(_service.GetMenu(restaurantId));
        }

        // ✅ GET SINGLE ITEM
        [HttpGet("item/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _service.GetMenuItemById(id));
        }

        // ✅ UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateMenuItemDto dto, IFormFile file)
        {
            await _service.UpdateMenuItem(id, dto, file);
            return Ok("Menu item updated");
        }

        // ✅ DELETE (SOFT)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteMenuItem(id);
            return Ok("Menu item deleted");
        }
    }
}
