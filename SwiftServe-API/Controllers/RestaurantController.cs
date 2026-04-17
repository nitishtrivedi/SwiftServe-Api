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

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_service.GetMyRestaurants());
        }
    }
}
