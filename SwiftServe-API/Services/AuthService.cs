using SwiftServe_API.Data;
using SwiftServe_API.DTOs;
using SwiftServe_API.Helpers;
using SwiftServe_API.Models;

namespace SwiftServe_API.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string> Register(RegisterDto dto)
        {
            if (_context.Users.Any(x => x.Email == dto.Email))
                throw new Exception("Email already exists");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = PasswordHelper.Hash(dto.Password),
                Role = "Customer",
                TenantId = null // ✅ Explicitly no tenant
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return JwtHelper.GenerateToken(user, _config);
        }

        public string Login(LoginDto dto)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.Email == dto.Email);

            if (user == null || user.PasswordHash != PasswordHelper.Hash(dto.Password))
                throw new Exception("Invalid credentials");

            return JwtHelper.GenerateToken(user, _config);
        }
    }
}
