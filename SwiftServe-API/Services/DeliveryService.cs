using Microsoft.AspNetCore.SignalR;
using SwiftServe_API.DTOs;
using SwiftServe_API.Enums;
using SwiftServe_API.Models;
using SwiftServe_API.Repositories;
using System.Security.Claims;

namespace SwiftServe_API.Services
{
    public class DeliveryService
    {
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<DeliveryLocation> _locationRepo;
        private readonly IHttpContextAccessor _http;
        private readonly IHubContext<TrackingHub> _hub;

        public DeliveryService(
            IRepository<Order> orderRepo,
            IRepository<DeliveryLocation> locationRepo,
            IHttpContextAccessor http,
            IHubContext<TrackingHub> hub)
        {
            _orderRepo = orderRepo;
            _locationRepo = locationRepo;
            _http = http;
            _hub = hub;
        }


        private int GetUserId()
        {
            var claim = _http.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(claim))
                throw new Exception("Unauthorized");

            return int.Parse(claim);
        }

        // ADMIN assigns delivery partner
        public async Task AssignDelivery(AssignDeliveryDto dto)
        {
            var order = await _orderRepo.GetById(dto.OrderId);

            if (order == null)// || order.TenantId != GetTenantId())
                throw new Exception("Unauthorized");

            order.DeliveryPartnerId = dto.DeliveryPartnerId;
            order.Status = OrderStatus.Accepted;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepo.Save();
        }

        // DELIVERY PARTNER updates location
        public async Task UpdateLocation(UpdateLocationDto dto, int orderId)
        {
            var location = new DeliveryLocation
            {
                DeliveryPartnerId = GetUserId(),
                Latitude = dto.Latitude,
                Longitude = dto.Longitude
            };

            await _locationRepo.Add(location);
            await _locationRepo.Save();

            // 🔥 Push live update
            await _hub.Clients.Group(orderId.ToString())
                .SendAsync("ReceiveLocation", dto.Latitude, dto.Longitude);
        }

        // DELIVERY PARTNER gets assigned orders
        public List<Order> GetMyDeliveries()
        {
            var userId = GetUserId();

            return _orderRepo.GetAll()
                .Where(x => x.DeliveryPartnerId == userId &&
                            x.Status != OrderStatus.Delivered &&
                            x.Status != OrderStatus.Cancelled)
                .ToList();
        }

        // DELIVERY updates order status
        public async Task UpdateOrderStatus(int orderId, string status)
        {
            var order = await _orderRepo.GetById(orderId);

            if (order == null || order.DeliveryPartnerId != GetUserId())
                throw new Exception("Unauthorized");

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepo.Save();
        }

        // CUSTOMER tracking (latest location)
        public DeliveryLocation GetLatestLocation(int deliveryPartnerId)
        {
            return _locationRepo.GetAll()
                .Where(x => x.DeliveryPartnerId == deliveryPartnerId)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault();
        }
    }
}
