using Razorpay.Api;
using SwiftServe_API.DTOs;
using SwiftServe_API.Models;
using SwiftServe_API.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace SwiftServe_API.Services
{
    public class RazorpayService
    {
        private readonly IConfiguration _config;
        private readonly IRepository<Models.Order> _orderRepo;

        public RazorpayService(IConfiguration config, IRepository<Models.Order> orderRepo)
        {
            _config = config;
            _orderRepo = orderRepo;
        }

        public object CreateOrder(int orderId)
        {
            var order = _orderRepo.GetAll().FirstOrDefault(x => x.Id == orderId);

            if (order == null)
                throw new Exception("Order not found");

            if (!order.IsPaymentEnabled)
                throw new Exception("Payment not required");

            var client = new RazorpayClient(
                _config["Razorpay:Key"],
                _config["Razorpay:Secret"]
            );

            var options = new Dictionary<string, object>
        {
            { "amount", (int)(order.TotalAmount * 100) }, // paise
            { "currency", "INR" },
            { "receipt", $"order_{order.Id}" }
        };

            var razorpayOrder = client.Order.Create(options);

            return new
            {
                key = _config["Razorpay:Key"],
                amount = order.TotalAmount,
                razorpayOrderId = razorpayOrder["id"].ToString()
            };
        }

        public async Task VerifyPayment(VerifyPaymentDto dto)
        {
            var order = await _orderRepo.GetById(dto.OrderId);

            if (order == null)
                throw new Exception("Order not found");

            // ✅ Prevent duplicate verification
            if (order.PaymentStatus == PaymentStatus.Paid)
                return;

            var secret = _config["Razorpay:Secret"];

            var payload = $"{dto.RazorpayOrderId}|{dto.RazorpayPaymentId}";

            var hash = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hashBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(payload));

            var generatedSignature = BitConverter
                .ToString(hashBytes)
                .Replace("-", "")
                .ToLower();

            if (generatedSignature != dto.RazorpaySignature)
            {
                order.PaymentStatus = PaymentStatus.Failed;
                await _orderRepo.Save();
                throw new Exception("Payment verification failed");
            }

            order.PaymentStatus = PaymentStatus.Paid;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepo.Save();
        }
    }
}
