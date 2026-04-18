using SwiftServe_API.DTOs;
using SwiftServe_API.Models;
using SwiftServe_API.Repositories;

namespace SwiftServe_API.Services
{
    public class MenuItemService
    {
        private readonly IRepository<MenuItem> _repo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Restaurant> _restaurantRepo;
        private readonly IHttpContextAccessor _http;
        private readonly CloudinaryService _cloudinary;

        public MenuItemService(
            IRepository<MenuItem> repo,
            IRepository<Category> categoryRepo,
            IRepository<Restaurant> restaurantRepo,
            IHttpContextAccessor http,
            CloudinaryService cloudinary)
        {
            _repo = repo;
            _categoryRepo = categoryRepo;
            _restaurantRepo = restaurantRepo;
            _http = http;
            _cloudinary = cloudinary;
        }

        private int GetTenantId()
        {
            var claim = _http.HttpContext?.User?.FindFirst("TenantId")?.Value;

            if (string.IsNullOrEmpty(claim))
                throw new Exception("Tenant not found");

            return int.Parse(claim);
        }

        // ✅ CREATE
        public async Task Create(CreateMenuItemDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Name required");

            if (dto.Price <= 0)
                throw new Exception("Invalid price");

            var restaurant = await _restaurantRepo.GetById(dto.RestaurantId);
            if (restaurant == null || restaurant.IsDeleted || restaurant.TenantId != GetTenantId())
                throw new Exception("Invalid restaurant");

            var category = await _categoryRepo.GetById(dto.CategoryId);
            if (category == null || category.IsDeleted || category.RestaurantId != dto.RestaurantId)
                throw new Exception("Invalid category");

            string imageUrl = null;

            if (dto.File != null)
            {
                imageUrl = await _cloudinary.UploadImage(dto.File);
            }

            var item = new MenuItem
            {
                RestaurantId = dto.RestaurantId,
                CategoryId = dto.CategoryId,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = imageUrl,
                IsAvailable = true,
                TenantId = GetTenantId(),
                CreatedAt = DateTime.UtcNow
            };

            await _repo.Add(item);
            await _repo.Save();
        }

        // ✅ GET MENU (PUBLIC)
        public List<MenuItemResponseDto> GetByRestaurant(int restaurantId)
        {
            return _repo.GetAll()
                .Where(x => x.RestaurantId == restaurantId && !x.IsDeleted && x.IsAvailable)
                .Select(x => new MenuItemResponseDto
                {
                    Id = x.Id,
                    RestaurantId = x.RestaurantId,
                    CategoryId = x.CategoryId,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    ImageUrl = x.ImageUrl,
                    IsAvailable = x.IsAvailable
                })
                .ToList();
        }

        // ✅ GET BY ID
        public async Task<MenuItemResponseDto> GetById(int id)
        {
            var item = await _repo.GetById(id);

            if (item == null || item.IsDeleted)
                throw new Exception("Item not found");

            return new MenuItemResponseDto
            {
                Id = item.Id,
                RestaurantId = item.RestaurantId,
                CategoryId = item.CategoryId,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                ImageUrl = item.ImageUrl,
                IsAvailable = item.IsAvailable
            };
        }

        // ✅ UPDATE
        public async Task Update(int id, UpdateMenuItemDto dto)
        {
            var item = await _repo.GetById(id);

            if (item == null || item.IsDeleted)
                throw new Exception("Item not found");

            var restaurant = await _restaurantRepo.GetById(item.RestaurantId);
            if (restaurant == null || restaurant.TenantId != GetTenantId())
                throw new Exception("Unauthorized");

            item.Name = dto.Name;
            item.Description = dto.Description;
            item.Price = dto.Price;
            item.IsAvailable = dto.IsAvailable;

            if (dto.File != null)
            {
                var imageUrl = await _cloudinary.UploadImage(dto.File);
                item.ImageUrl = imageUrl;
            }

            item.UpdatedAt = DateTime.UtcNow;

            await _repo.Save();
        }

        // ✅ DELETE (SOFT)
        public async Task Delete(int id)
        {
            var item = await _repo.GetById(id);

            if (item == null)
                throw new Exception("Item not found");

            var restaurant = await _restaurantRepo.GetById(item.RestaurantId);
            if (restaurant == null || restaurant.TenantId != GetTenantId())
                throw new Exception("Unauthorized");

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;

            await _repo.Save();
        }
    }
}
