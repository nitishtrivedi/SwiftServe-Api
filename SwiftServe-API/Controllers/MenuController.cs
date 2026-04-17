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

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateMenuItemDto dto, IFormFile file)
        {
            await _service.CreateMenuItem(dto, file);
            return Ok("Menu item created");
        }

        [HttpGet("{restaurantId}")]
        [AllowAnonymous]
        public IActionResult GetMenu(int restaurantId)
        {
            return Ok(_service.GetMenu(restaurantId));
        }
    }
}
