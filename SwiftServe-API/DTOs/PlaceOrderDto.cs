namespace SwiftServe_API.DTOs
{
    public class PlaceOrderDto
    {
        public int RestaurantId { get; set; }
        public string PaymentMethod { get; set; }

        public double DeliveryLatitude { get; set; }
        public double DeliveryLongitude { get; set; }
    }

    public static class PaymentMethod
    {
        public const string COD = "COD";
        public const string Razorpay = "Razorpay";
    }

    public static class PaymentStatus
    {
        public const string Pending = "Pending";
        public const string Paid = "Paid";
        public const string Failed = "Failed";
    }
}
