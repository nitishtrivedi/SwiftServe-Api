using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftServe_API.Services;
using System.Security.Claims;

namespace SwiftServe_API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _service;
        private readonly IHttpContextAccessor _http;

        public NotificationController(NotificationService service, IHttpContextAccessor http)
        {
            _service = service;
            _http = http;
        }

        private int GetUserId()
        {
            return int.Parse(_http.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_service.GetUserNotifications(GetUserId()));
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkRead(int id)
        {
            await _service.MarkAsRead(id);
            return Ok();
        }
    }
}
