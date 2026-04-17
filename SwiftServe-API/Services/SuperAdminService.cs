using SwiftServe_API.DTOs;
using SwiftServe_API.Helpers;
using SwiftServe_API.Models;
using SwiftServe_API.Repositories;

namespace SwiftServe_API.Services
{
    public class SuperAdminService
    {
        private readonly IRepository<Tenant> _tenantRepo;
        private readonly IRepository<User> _userRepo;

        public SuperAdminService(IRepository<Tenant> tenantRepo, IRepository<User> userRepo)
        {
            _tenantRepo = tenantRepo;
            _userRepo = userRepo;
        }

        public async Task CreateTenantWithAdmin(CreateTenantAdminDto dto)
        {
            // Check email uniqueness
            if (_userRepo.GetAll().Any(x => x.Email == dto.AdminEmail))
                throw new Exception("Admin email already exists");

            // STEP 1: Create Admin
            var admin = new User
            {
                Name = dto.AdminName,
                Email = dto.AdminEmail,
                PhoneNumber = dto.AdminPhone,
                PasswordHash = PasswordHelper.Hash(dto.Password),
                Role = "Admin"
            };

            await _userRepo.Add(admin);
            await _userRepo.Save();

            // STEP 2: Create Tenant
            var tenant = new Tenant
            {
                Name = dto.TenantName,
                OwnerId = admin.Id,
                TenantId = admin.Id // self-mapped for consistency
            };

            await _tenantRepo.Add(tenant);
            await _tenantRepo.Save();

            // STEP 3: Update Admin with TenantId
            admin.TenantId = tenant.Id;

            await _userRepo.Save();
        }
    }
}
