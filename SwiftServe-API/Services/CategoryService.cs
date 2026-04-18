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

        // ✅ CREATE
        public async Task Create(CreateCategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Category name required");

            var restaurant = await _restaurantRepo.GetById(dto.RestaurantId);

            if (restaurant == null || restaurant.IsDeleted || restaurant.TenantId != GetTenantId())
                throw new Exception("Invalid restaurant");

            var category = new Category
            {
                RestaurantId = dto.RestaurantId,
                Name = dto.Name,
                DisplayOrder = dto.DisplayOrder,
                TenantId = GetTenantId(),
                CreatedAt = DateTime.UtcNow
            };

            await _repo.Add(category);
            await _repo.Save();
        }

        // ✅ GET ALL BY RESTAURANT (Admin + Customer)
        public List<CategoryResponseDto> GetByRestaurant(int restaurantId)
        {
            return _repo.GetAll()
                .Where(x => x.RestaurantId == restaurantId && !x.IsDeleted)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new CategoryResponseDto
                {
                    Id = x.Id,
                    RestaurantId = x.RestaurantId,
                    Name = x.Name,
                    DisplayOrder = x.DisplayOrder
                })
                .ToList();
        }

        // ✅ GET BY ID
        public async Task<CategoryResponseDto> GetById(int id)
        {
            var category = await _repo.GetById(id);

            if (category == null || category.IsDeleted)
                throw new Exception("Category not found");

            return new CategoryResponseDto
            {
                Id = category.Id,
                RestaurantId = category.RestaurantId,
                Name = category.Name,
                DisplayOrder = category.DisplayOrder
            };
        }

        // ✅ UPDATE
        public async Task Update(int id, UpdateCategoryDto dto)
        {
            var category = await _repo.GetById(id);

            if (category == null || category.IsDeleted)
                throw new Exception("Category not found");

            // 🔒 Tenant check via restaurant
            var restaurant = await _restaurantRepo.GetById(category.RestaurantId);

            if (restaurant == null || restaurant.TenantId != GetTenantId())
                throw new Exception("Unauthorized");

            category.Name = dto.Name;
            category.DisplayOrder = dto.DisplayOrder;
            category.UpdatedAt = DateTime.UtcNow;

            await _repo.Save();
        }

        // ✅ DELETE (SOFT)
        public async Task Delete(int id)
        {
            var category = await _repo.GetById(id);

            if (category == null)
                throw new Exception("Category not found");

            var restaurant = await _restaurantRepo.GetById(category.RestaurantId);

            if (restaurant == null || restaurant.TenantId != GetTenantId())
                throw new Exception("Unauthorized");

            category.IsDeleted = true;
            category.UpdatedAt = DateTime.UtcNow;

            await _repo.Save();
        }
    }
}
