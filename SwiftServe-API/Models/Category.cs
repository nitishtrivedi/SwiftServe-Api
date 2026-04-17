namespace SwiftServe_API.Models
{
    public class Category : BaseEntity
    {
        public int RestaurantId { get; set; }

        public string Name { get; set; }

        public int DisplayOrder { get; set; }
    }
}
