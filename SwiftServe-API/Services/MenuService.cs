using SwiftServe_API.DTOs;
using SwiftServe_API.Models;
using SwiftServe_API.Repositories;

namespace SwiftServe_API.Services
{
    public class MenuService
    {
        private readonly IRepository<MenuItem> _repo;
        private readonly IRepository<Restaurant> _restaurantRepo;
        private readonly CloudinaryService _cloudinary;
        private readonly IHttpContextAccessor _http;

        public MenuService(
            IRepository<MenuItem> repo,
            IRepository<Restaurant> restaurantRepo,
            CloudinaryService cloudinary,
            IHttpContextAccessor http)
        {
            _repo = repo;
            _restaurantRepo = restaurantRepo;
            _cloudinary = cloudinary;
            _http = http;
        }

        private int GetTenantId()
        {
            var claim = _http.HttpContext?.User?.FindFirst("TenantId")?.Value;

            if (string.IsNullOrEmpty(claim))
                throw new Exception("Tenant not found");

            return int.Parse(claim);
        }

        public async Task CreateMenuItem(CreateMenuItemDto dto, IFormFile file)
        {
            var restaurant = await _restaurantRepo.GetById(dto.RestaurantId);

            if (restaurant == null || restaurant.TenantId != GetTenantId())
                throw new Exception("Unauthorized");

            var imageUrl = await _cloudinary.UploadImage(file);

            var item = new MenuItem
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                RestaurantId = dto.RestaurantId,
                ImageUrl = imageUrl,
                TenantId = GetTenantId()
            };

            await _repo.Add(item);
            await _repo.Save();
        }

        public List<MenuItem> GetMenu(int restaurantId)
        {
            return _repo.GetAll()
                .Where(x => x.RestaurantId == restaurantId && x.IsAvailable)
                .ToList();
        }
    }
}
