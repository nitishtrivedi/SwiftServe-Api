using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftServe_API.DTOs;
using SwiftServe_API.Services;

namespace SwiftServe_API.Controllers
{
    [ApiController]
    [Route("api/superadmin")]
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : ControllerBase
    {
        private readonly SuperAdminService _service;

        public SuperAdminController(SuperAdminService service)
        {
            _service = service;
        }

        [HttpPost("create-tenant")]
        public async Task<IActionResult> CreateTenant(CreateTenantAdminDto dto)
        {
            await _service.CreateTenantWithAdmin(dto);
            return Ok("Tenant and Admin created successfully");
        }
    }
}
