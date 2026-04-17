using SwiftServe_API.DTOs;
using SwiftServe_API.Models;
using SwiftServe_API.Repositories;
using System.Security.Claims;

namespace SwiftServe_API.Services
{
    public class CartService
    {
        private readonly IRepository<Cart> _cartRepo;
        private readonly IRepository<CartItem> _itemRepo;
        private readonly IRepository<MenuItem> _menuRepo;
        private readonly IHttpContextAccessor _http;

        public CartService(
            IRepository<Cart> cartRepo,
            IRepository<CartItem> itemRepo,
            IRepository<MenuItem> menuRepo,
            IHttpContextAccessor http)
        {
            _cartRepo = cartRepo;
            _itemRepo = itemRepo;
            _menuRepo = menuRepo;
            _http = http;
        }

        private int GetUserId()
        {
            var claim = _http.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(claim))
                throw new Exception("Unauthorized");

            return int.Parse(claim);
        }

        public async Task AddToCart(AddToCartDto dto)
        {
            // ✅ VALIDATION
            if (dto.Quantity <= 0)
                throw new Exception("Quantity must be greater than 0");

            if (dto.MenuItemId <= 0 || dto.RestaurantId <= 0)
                throw new Exception("Invalid request");

            var userId = GetUserId();

            var menuItem = await _menuRepo.GetById(dto.MenuItemId);

            if (menuItem == null)
                throw new Exception("Menu item not found");

            if (!menuItem.IsAvailable)
                throw new Exception("Item is not available");

            if (menuItem.RestaurantId != dto.RestaurantId)
                throw new Exception("Item does not belong to restaurant");

            var cart = _cartRepo.GetAll()
                .FirstOrDefault(x => x.CustomerId == userId && !x.IsCheckedOut);

            if (cart != null && cart.RestaurantId != dto.RestaurantId)
                throw new Exception("Cannot mix items from different restaurants");

            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = userId,
                    RestaurantId = dto.RestaurantId
                };

                await _cartRepo.Add(cart);
                await _cartRepo.Save();
            }

            var existingItem = _itemRepo.GetAll()
                .FirstOrDefault(x => x.CartId == cart.Id && x.MenuItemId == dto.MenuItemId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                await _itemRepo.Add(new CartItem
                {
                    CartId = cart.Id,
                    MenuItemId = dto.MenuItemId,
                    Quantity = dto.Quantity
                });
            }

            await _itemRepo.Save();
        }

        public async Task<object> GetCart()
        {
            var userId = GetUserId();

            var cart = _cartRepo.GetAll()
                .FirstOrDefault(x => x.CustomerId == userId && !x.IsCheckedOut);

            if (cart == null)
                return null;

            var items = _itemRepo.GetAll()
                .Where(x => x.CartId == cart.Id)
                .ToList();

            decimal total = 0;

            var resultItems = new List<object>();

            foreach (var item in items)
            {
                var menuItem = await _menuRepo.GetById(item.MenuItemId);

                var itemTotal = menuItem.Price * item.Quantity;
                total += itemTotal;

                resultItems.Add(new
                {
                    item.MenuItemId,
                    menuItem.Name,
                    menuItem.Price,
                    item.Quantity,
                    itemTotal
                });
            }

            return new
            {
                cart.Id,
                cart.RestaurantId,
                items = resultItems,
                totalAmount = total
            };
        }

        public async Task ClearCart(int cartId)
        {
            var items = _itemRepo.GetAll().Where(x => x.CartId == cartId);

            foreach (var item in items)
                item.IsDeleted = true;

            var cart = await _cartRepo.GetById(cartId);
            cart.IsCheckedOut = true;

            await _cartRepo.Save();
        }
    }
}
