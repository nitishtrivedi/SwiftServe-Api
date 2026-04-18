using SwiftServe_API.DTOs;
using SwiftServe_API.Models;
using SwiftServe_API.Repositories;
using System.Security.Claims;

namespace SwiftServe_API.Services
{
    //Admin Service (Restaurant Management)
    public class RestaurantService
    {
        private readonly IRepository<Restaurant> _repo;
        private readonly IHttpContextAccessor _http;

        public RestaurantService(IRepository<Restaurant> repo, IHttpContextAccessor http)
        {
            _repo = repo;
            _http = http;
        }

        private int GetTenantId()
        {
            var tenantId = _http.HttpContext.User.Claims
                .First(x => x.Type == "TenantId").Value;

            return int.Parse(tenantId);
        }

        private int GetUserId()
        {
            var userId = _http.HttpContext.User.Claims
                .First(x => x.Type == ClaimTypes.NameIdentifier).Value;

            return int.Parse(userId);
        }

        public async Task CreateRestaurant(CreateRestaurantDto dto)
        {
            var restaurant = new Restaurant
            {
                Name = dto.Name,
                Address = dto.Address,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                TenantId = GetTenantId(),
                OwnerId = GetUserId()
            };

            await _repo.Add(restaurant);
            await _repo.Save();
        }

        public List<Restaurant> GetMyRestaurants()
        {
            var tenantId = GetTenantId();

            return _repo.GetAll()
                .Where(x => x.TenantId == tenantId)
                .ToList();
        }

        // ✅ GET ALL (Admin)
        public List<RestaurantResponseDto> GetAll()
        {
            var tenantId = GetTenantId();

            return [.. _repo.GetAll()
                .Where(x => x.TenantId == tenantId && !x.IsDeleted)
                .Select(x => new RestaurantResponseDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude
                })];
        }

        // ✅ GET BY ID
        public async Task<RestaurantResponseDto> GetById(int id)
        {
            var restaurant = await _repo.GetById(id);

            if (restaurant == null || restaurant.IsDeleted || restaurant.TenantId != GetTenantId())
                throw new Exception("Restaurant not found");

            return new RestaurantResponseDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Address = restaurant.Address,
                Latitude = restaurant.Latitude,
                Longitude = restaurant.Longitude
            };
        }

        // ✅ UPDATE
        public async Task Update(int id, UpdateRestaurantDto dto)
        {
            var restaurant = await _repo.GetById(id);

            if (restaurant == null || restaurant.IsDeleted || restaurant.TenantId != GetTenantId())
                throw new Exception("Unauthorized");

            restaurant.Name = dto.Name;
            restaurant.Address = dto.Address;
            restaurant.Latitude = dto.Latitude;
            restaurant.Longitude = dto.Longitude;
            restaurant.UpdatedAt = DateTime.UtcNow;

            await _repo.Save();
        }

        // ✅ DELETE (SOFT DELETE)
        public async Task Delete(int id)
        {
            var restaurant = await _repo.GetById(id);

            if (restaurant == null || restaurant.TenantId != GetTenantId())
                throw new Exception("Unauthorized");

            restaurant.IsDeleted = true;
            restaurant.UpdatedAt = DateTime.UtcNow;

            await _repo.Save();
        }
    }
}
