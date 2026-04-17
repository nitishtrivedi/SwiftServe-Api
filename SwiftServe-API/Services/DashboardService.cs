using SwiftServe_API.DTOs;
using SwiftServe_API.Models;
using SwiftServe_API.Repositories;

namespace SwiftServe_API.Services
{
    public class DashboardService
    {
        private readonly IRepository<Order> _orderRepo;
        private readonly IHttpContextAccessor _http;

        public DashboardService(IRepository<Order> orderRepo,
                                IHttpContextAccessor http)
        {
            _orderRepo = orderRepo;
            _http = http;
        }

        private int GetTenantId()
        {
            var claim = _http.HttpContext?.User?.FindFirst("TenantId")?.Value;

            if (string.IsNullOrEmpty(claim))
                throw new Exception("Tenant not found");

            return int.Parse(claim);
        }

        public object GetStats()
        {
            var tenantId = GetTenantId();

            var orders = _orderRepo.GetAll()
                .Where(x => x.TenantId == tenantId)
                .ToList();

            var today = DateTime.UtcNow.Date;

            var todayOrders = orders.Where(x => x.CreatedAt.Date == today).ToList();

            return new
            {
                totalOrders = orders.Count,
                totalRevenue = orders
                    .Where(x => x.PaymentStatus == PaymentStatus.Paid)
                    .Sum(x => x.TotalAmount),

                todayOrders = todayOrders.Count,

                todayRevenue = todayOrders
                    .Where(x => x.PaymentStatus == PaymentStatus.Paid)
                    .Sum(x => x.TotalAmount)
            };
        }
    }
}
