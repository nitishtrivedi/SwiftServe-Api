namespace SwiftServe_API.Models
{
    public class DeliveryLocation : BaseEntity
    {
        public int DeliveryPartnerId { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
