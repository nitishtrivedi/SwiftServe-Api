using SwiftServe_API.Enums;
using SwiftServe_API.Models;
using SwiftServe_API.Repositories;
using System.Security.Claims;

namespace SwiftServe_API.Services
{
    public class AdminOrderService
    {
        private readonly IRepository<Order> _orderRepo;
        private readonly IHttpContextAccessor _http;
        private readonly NotificationService _notification;
        private readonly IRepository<User> _userRepo;

        public AdminOrderService(
            IRepository<Order> orderRepo,
            IHttpContextAccessor http,
            NotificationService notification,
            IRepository<User> userRepo)
        {
            _orderRepo = orderRepo;
            _http = http;
            _notification = notification;
            _userRepo = userRepo;
        }

        private int GetTenantId()
        {
            var claim = _http.HttpContext?.User?.FindFirst("TenantId")?.Value;

            if (string.IsNullOrEmpty(claim))
                throw new Exception("Tenant not found");

            return int.Parse(claim);
        }

        private int GetUserId()
        {
            var claim = _http.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(claim))
                throw new Exception("Unauthorized");

            return int.Parse(claim);
        }

        // ✅ Get all orders for this admin's tenant
        public List<Order> GetOrders()
        {
            var tenantId = GetTenantId();

            return _orderRepo.GetAll()
                .Where(x => x.TenantId == tenantId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }

        // ✅ Get specific order details
        public async Task<Order> GetOrderById(int orderId)
        {
            var order = await _orderRepo.GetById(orderId);

            if (order == null || order.TenantId != GetTenantId())
                throw new Exception("Unauthorized");

            return order;
        }

        // ✅ Update order status (core flow)
        public async Task UpdateStatus(int orderId, string status)
        {
            var order = await _orderRepo.GetById(orderId);

            if (order == null || order.TenantId != GetTenantId())
                throw new Exception("Unauthorized");

            // 🔒 Prevent invalid transitions (basic guard)
            if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
                throw new Exception("Order already completed");

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepo.Save();

            // 🔔 Notify customer
            await _notification.Create(
                order.CustomerId,
                "Order Update",
                $"Your order #{order.Id} is now {status}"
            );
        }

        // ✅ Cancel order
        public async Task CancelOrder(int orderId)
        {
            var order = await _orderRepo.GetById(orderId);

            if (order == null || order.TenantId != GetTenantId())
                throw new Exception("Unauthorized");

            if (order.Status == OrderStatus.Delivered)
                throw new Exception("Cannot cancel delivered order");

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepo.Save();

            // 🔔 Notify customer
            await _notification.Create(
                order.CustomerId,
                "Order Cancelled",
                $"Your order #{order.Id} has been cancelled"
            );
        }
    }
}
