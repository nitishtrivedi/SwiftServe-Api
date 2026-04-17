using SwiftServe_API.DTOs;
using SwiftServe_API.Models;
using SwiftServe_API.Repositories;

namespace SwiftServe_API.Services
{
    public class CategoryService
    {
        private readonly IRepository<Category> _repo;
        private readonly IHttpContextAccessor _http;
        private readonly IRepository<Restaurant> _restaurantRepo;

        public CategoryService(
            IRepository<Category> repo,
            IHttpContextAccessor http,
            IRepository<Restaurant> restaurantRepo)
        {
            _repo = repo;
            _http = http;
            _restaurantRepo = restaurantRepo;
        }

        private int GetTenantId()
        {
            var claim = _http.HttpContext?.User?.FindFirst("TenantId")?.Value;

            if (string.IsNullOrEmpty(claim))
                throw new Exception("Tenant not found");

            return int.Parse(claim);
        }

        public async Task CreateCategory(CreateCategoryDto dto)
        {
            var restaurant = await _restaurantRepo.GetById(dto.RestaurantId);

            if (restaurant == null || restaurant.TenantId != GetTenantId())
                throw new Exception("Unauthorized");

            var category = new Category
            {
                Name = dto.Name,
                DisplayOrder = dto.DisplayOrder,
                RestaurantId = dto.RestaurantId,
                TenantId = GetTenantId()
            };

            await _repo.Add(category);
            await _repo.Save();
        }

        public List<Category> GetByRestaurant(int restaurantId)
        {
            return _repo.GetAll()
                .Where(x => x.RestaurantId == restaurantId)
                .OrderBy(x => x.DisplayOrder)
                .ToList();
        }
    }
}
