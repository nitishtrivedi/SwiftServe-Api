using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftServe_API.DTOs;
using SwiftServe_API.Services;

namespace SwiftServe_API.Controllers
{
    [ApiController]
    [Route("api/admin/restaurants")]
    [Authorize(Roles = "Admin")]
    public class RestaurantController : ControllerBase
    {
        private readonly RestaurantService _service;

        public RestaurantController(RestaurantService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRestaurantDto dto)
        {
            await _service.CreateRestaurant(dto);
            return Ok("Restaurant created");
        }

        // ✅ GET ALL
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }

        // ✅ GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _service.GetById(id));
        }

        // ✅ UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateRestaurantDto dto)
        {
            await _service.Update(id, dto);
            return Ok("Updated");
        }

        // ✅ DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id);
            return Ok("Deleted");
        }
    }
}
