namespace SwiftServe_API.Models
{
    public class Cart : BaseEntity
    {
        public int CustomerId { get; set; }

        public int RestaurantId { get; set; }

        public bool IsCheckedOut { get; set; } = false;
    }
}
