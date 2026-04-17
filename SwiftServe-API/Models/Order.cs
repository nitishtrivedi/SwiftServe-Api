using System.ComponentModel.DataAnnotations;

namespace SwiftServe_API.Models
{
    public class Order : BaseEntity
    {
        public int CustomerId { get; set; }
        public int RestaurantId { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; }

        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }

        public bool IsPaymentEnabled { get; set; }

        public double DeliveryLatitude { get; set; }
        public double DeliveryLongitude { get; set; }

        public int? DeliveryPartnerId { get; set; }
    }
}
