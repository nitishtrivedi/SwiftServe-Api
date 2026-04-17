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
    }
}
