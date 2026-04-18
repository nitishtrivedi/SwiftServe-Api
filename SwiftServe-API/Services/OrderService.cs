using SwiftServe_API.DTOs;
using SwiftServe_API.Enums;
using SwiftServe_API.Models;
using SwiftServe_API.Repositories;
using System.Security.Claims;

namespace SwiftServe_API.Services
{
    public class OrderService
    {
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<OrderItem> _orderItemRepo;
        private readonly IRepository<Cart> _cartRepo;
        private readonly IRepository<CartItem> _cartItemRepo;
        private readonly IRepository<MenuItem> _menuRepo;
        private readonly IRepository<Restaurant> _restaurantRepo;
        private readonly CartService _cartService;
        private readonly IHttpContextAccessor _http;

        public OrderService(
            IRepository<Order> orderRepo,
            IRepository<OrderItem> orderItemRepo,
            IRepository<Cart> cartRepo,
            IRepository<CartItem> cartItemRepo,
            IRepository<MenuItem> menuRepo,
            IRepository<Restaurant> restaurantRepo,
            CartService cartService,
            IHttpContextAccessor http)
        {
            _orderRepo = orderRepo;
            _orderItemRepo = orderItemRepo;
            _cartRepo = cartRepo;
            _cartItemRepo = cartItemRepo;
            _menuRepo = menuRepo;
            _restaurantRepo = restaurantRepo;
            _cartService = cartService;
            _http = http;
        }

        private int GetUserId()
        {
            var userId = _http.HttpContext?.User?.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            return int.Parse(userId);
        }

        // ✅ PLACE ORDER
        public async Task<object> PlaceOrder(PlaceOrderDto dto)
        {
            var userId = GetUserId();

            var cart = _cartRepo.GetAll()
                .FirstOrDefault(x => x.CustomerId == userId && !x.IsCheckedOut);

            if (cart == null)
                throw new Exception("Cart is empty");

            var cartItems = _cartItemRepo.GetAll()
                .Where(x => x.CartId == cart.Id && !x.IsDeleted)
                .ToList();

            if (!cartItems.Any())
                throw new Exception("Cart has no items");

            decimal totalAmount = 0;

            foreach (var item in cartItems)
            {
                var menuItem = await _menuRepo.GetById(item.MenuItemId);

                if (menuItem == null)
                    throw new Exception($"Menu item {item.MenuItemId} not found");

                // ✅ FIX: use correct property name
                totalAmount += menuItem.Price * item.Quantity; // <-- CHANGE if your field name differs
            }

            var order = new Order
            {
                CustomerId = userId,
                TotalAmount = totalAmount,
                Status = OrderStatus.Pending, // ✅ ENUM FIX
                DeliveryAddress = dto.DeliveryAddress,
                CreatedAt = DateTime.UtcNow
            };

            await _orderRepo.Add(order);
            await _orderRepo.Save();

            foreach (var item in cartItems)
            {
                var menuItem = await _menuRepo.GetById(item.MenuItemId);

                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    MenuItemId = item.MenuItemId,
                    Quantity = item.Quantity,
                    Price = menuItem.Price // ✅ FIX
                };

                await _orderItemRepo.Add(orderItem);
            }

            await _orderItemRepo.Save();

            // ✅ CLEAR CART
            foreach (var item in cartItems)
                item.IsDeleted = true;

            cart.IsCheckedOut = true;
            cart.UpdatedAt = DateTime.UtcNow;

            await _cartRepo.Save();

            return new
            {
                order.Id,
                order.TotalAmount,
                order.Status
            };
        }

        // ✅ CANCEL ORDER
        public async Task CancelOrder(int orderId)
        {
            var userId = GetUserId();

            var order = _orderRepo.GetAll()
                .FirstOrDefault(x => x.Id == orderId && x.CustomerId == userId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.Status == "Cancelled")
                throw new Exception("Order already cancelled");

            if (order.Status != "Placed")
                throw new Exception("Only placed orders can be cancelled");

            order.Status = "Cancelled";
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepo.Save();
        }

        // ✅ GET MY ORDERS
        public async Task<List<object>> GetMyOrders()
        {
            var userId = GetUserId();

            var orders = _orderRepo.GetAll()
                .Where(x => x.CustomerId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var result = new List<object>();

            foreach (var order in orders)
            {
                var items = _orderItemRepo.GetAll()
                    .Where(x => x.OrderId == order.Id)
                    .ToList();

                result.Add(new
                {
                    order.Id,
                    order.TotalAmount,
                    order.Status,
                    order.DeliveryAddress,
                    order.CreatedAt,
                    Items = items.Select(i => new
                    {
                        i.MenuItemId,
                        i.Quantity,
                        i.Price
                    })
                });
            }

            return result;
        }

        // ✅ GET ORDER BY ID
        public async Task<object> GetOrderById(int orderId)
        {
            var userId = GetUserId();

            var order = _orderRepo.GetAll()
                .FirstOrDefault(x => x.Id == orderId && x.CustomerId == userId);

            if (order == null)
                throw new Exception("Order not found");

            var items = _orderItemRepo.GetAll()
                .Where(x => x.OrderId == order.Id)
                .ToList();

            return new
            {
                order.Id,
                order.TotalAmount,
                order.Status,
                order.DeliveryAddress,
                order.CreatedAt,
                Items = items.Select(i => new
                {
                    i.MenuItemId,
                    i.Quantity,
                    i.Price
                })
            };
        }
    }
}
