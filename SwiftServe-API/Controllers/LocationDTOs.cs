namespace SwiftServe_API.Controllers
{
    public class UpdateLocationDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class AssignDeliveryDto
    {
        public int OrderId { get; set; }
        public int DeliveryPartnerId { get; set; }
    }
}
