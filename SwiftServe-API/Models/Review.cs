namespace SwiftServe_API.Models
{
    public class Review : BaseEntity
    {
        public int CustomerId { get; set; }
        public int RestaurantId { get; set; }

        public int Rating { get; set; } // 1–5
        public string Comment { get; set; }
    }
}
