using Microsoft.AspNetCore.Mvc;
using SwiftServe_API.DTOs;
using SwiftServe_API.Services;

namespace SwiftServe_API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _service;

        public AuthController(AuthService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var token = await _service.Register(dto);
            return Ok(new { token });
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var token = _service.Login(dto);
            return Ok(new { token });
        }
    }
}
