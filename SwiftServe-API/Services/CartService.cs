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

        // ✅ ADD TO CART
        public async Task AddToCart(AddToCartDto dto)
        {
            if (dto.Quantity <= 0)
                throw new Exception("Invalid quantity");

            var userId = GetUserId();

            var menuItem = await _menuRepo.GetById(dto.MenuItemId);

            if (menuItem == null || !menuItem.IsAvailable)
                throw new Exception("Item not available");

            var cart = _cartRepo.GetAll()
                .FirstOrDefault(x => x.CustomerId == userId && !x.IsCheckedOut);

            // ❌ Prevent mixing restaurants
            if (cart != null && cart.RestaurantId != dto.RestaurantId)
                throw new Exception("Cannot mix items from different restaurants");

            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = userId,
                    RestaurantId = dto.RestaurantId,
                    CreatedAt = DateTime.UtcNow
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

        // ✅ GET CART
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
            var result = new List<object>();

            foreach (var item in items)
            {
                var menuItem = await _menuRepo.GetById(item.MenuItemId);

                if (menuItem == null) continue;

                var itemTotal = menuItem.Price * item.Quantity;
                total += itemTotal;

                result.Add(new
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
                items = result,
                totalAmount = total
            };
        }

        // ✅ CLEAR CART
        public async Task ClearUserCart()
        {
            var userId = GetUserId();

            var cart = _cartRepo.GetAll()
                .FirstOrDefault(x => x.CustomerId == userId && !x.IsCheckedOut);

            if (cart == null)
                return;

            var items = _itemRepo.GetAll()
                .Where(x => x.CartId == cart.Id);

            foreach (var item in items)
                item.IsDeleted = true;

            cart.IsCheckedOut = true;
            cart.UpdatedAt = DateTime.UtcNow;

            await _cartRepo.Save();
        }

        public async Task UpdateCartItem(UpdateCartItemDto dto)
        {
            var userId = GetUserId();

            var cart = _cartRepo.GetAll()
                .FirstOrDefault(x => x.CustomerId == userId && !x.IsCheckedOut);

            if (cart == null)
                throw new Exception("Cart not found");

            var item = _itemRepo.GetAll()
                .FirstOrDefault(x => x.CartId == cart.Id && x.MenuItemId == dto.MenuItemId);

            if (item == null)
                throw new Exception("Item not found in cart");

            if (dto.Quantity <= 0)
            {
                item.IsDeleted = true;
            }
            else
            {
                item.Quantity = dto.Quantity;
            }

            await _itemRepo.Save();
        }

        public async Task RemoveItem(int menuItemId)
        {
            var userId = GetUserId();

            var cart = _cartRepo.GetAll()
                .FirstOrDefault(x => x.CustomerId == userId && !x.IsCheckedOut);

            if (cart == null)
                throw new Exception("Cart not found");

            var item = _itemRepo.GetAll()
                .FirstOrDefault(x => x.CartId == cart.Id && x.MenuItemId == menuItemId);

            if (item == null)
                throw new Exception("Item not found");

            item.IsDeleted = true;

            await _itemRepo.Save();
        }
    }
}
