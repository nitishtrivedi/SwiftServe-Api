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
        private readonly IRepository<OrderItem> _itemRepo;
        private readonly IRepository<Cart> _cartRepo;
        private readonly IRepository<CartItem> _cartItemRepo;
        private readonly IRepository<MenuItem> _menuRepo;
        private readonly IRepository<Restaurant> _restaurantRepo;
        private readonly CartService _cartService;
        private readonly IHttpContextAccessor _http;

        public OrderService(
            IRepository<Order> orderRepo,
            IRepository<OrderItem> itemRepo,
            IRepository<Cart> cartRepo,
            IRepository<CartItem> cartItemRepo,
            IRepository<MenuItem> menuRepo,
            IRepository<Restaurant> restaurantRepo,
            CartService cartService,
            IHttpContextAccessor http)
        {
            _orderRepo = orderRepo;
            _itemRepo = itemRepo;
            _cartRepo = cartRepo;
            _cartItemRepo = cartItemRepo;
            _menuRepo = menuRepo;
            _restaurantRepo = restaurantRepo;
            _cartService = cartService;
            _http = http;
        }

        private int GetUserId()
        {
            var claim = _http.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(claim))
                throw new Exception("Unauthorized");

            return int.Parse(claim);
        }

        public async Task<object> PlaceOrder(PlaceOrderDto dto)
        {
            var userId = GetUserId();

            var cart = _cartRepo.GetAll()
                .FirstOrDefault(x => x.CustomerId == userId && !x.IsCheckedOut);

            if (cart == null || cart.RestaurantId != dto.RestaurantId)
                throw new Exception("Invalid cart");

            var cartItems = _cartItemRepo.GetAll()
                .Where(x => x.CartId == cart.Id)
                .ToList();

            if (!cartItems.Any())
                throw new Exception("Cart empty");

            var restaurant = await _restaurantRepo.GetById(dto.RestaurantId);

            decimal total = 0;

            var order = new Order
            {
                CustomerId = userId,
                RestaurantId = dto.RestaurantId,
                Status = OrderStatus.Pending,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = PaymentStatus.Pending,
                DeliveryLatitude = dto.DeliveryLatitude,
                DeliveryLongitude = dto.DeliveryLongitude,
                TenantId = restaurant.TenantId
            };

            await _orderRepo.Add(order);
            await _orderRepo.Save();

            foreach (var item in cartItems)
            {
                var menuItem = await _menuRepo.GetById(item.MenuItemId);

                var itemTotal = menuItem.Price * item.Quantity;
                total += itemTotal;

                await _itemRepo.Add(new OrderItem
                {
                    OrderId = order.Id,
                    MenuItemId = item.MenuItemId,
                    Quantity = item.Quantity,
                    Price = menuItem.Price,
                    TenantId = restaurant.TenantId
                });
            }

            order.TotalAmount = total;

            order.IsPaymentEnabled = dto.PaymentMethod == PaymentMethod.Razorpay;

            await _itemRepo.Save();
            await _orderRepo.Save();

            await _cartService.ClearCart(cart.Id);

            return new
            {
                orderId = order.Id,
                totalAmount = total,
                paymentRequired = order.IsPaymentEnabled
            };
        }
    }
}
