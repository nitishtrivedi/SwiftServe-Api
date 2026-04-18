using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftServe_API.DTOs;
using SwiftServe_API.Services;

namespace SwiftServe_API.Controllers
{
    [ApiController]
    [Route("api/admin/categories")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _service;

        public CategoryController(CategoryService service)
        {
            _service = service;
        }

        // ✅ CREATE (Admin)
        [HttpPost("admin/categories")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            await _service.Create(dto);
            return Ok("Category created");
        }

        // ✅ GET BY RESTAURANT (Public / Customer)
        [HttpGet("categories/{restaurantId}")]
        public IActionResult GetByRestaurant(int restaurantId)
        {
            return Ok(_service.GetByRestaurant(restaurantId));
        }

        // ✅ GET BY ID
        [HttpGet("categories/detail/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _service.GetById(id));
        }

        // ✅ UPDATE (Admin)
        [HttpPut("admin/categories/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateCategoryDto dto)
        {
            await _service.Update(id, dto);
            return Ok("Updated");
        }

        // ✅ DELETE (Admin)
        [HttpDelete("admin/categories/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id);
            return Ok("Deleted");
        }
    }
}
