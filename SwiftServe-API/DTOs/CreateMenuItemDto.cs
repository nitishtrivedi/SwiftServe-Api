namespace SwiftServe_API.DTOs
{
    public class CreateMenuItemDto
    {
        public int RestaurantId { get; set; }
        public int CategoryId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public decimal Price { get; set; }
    }
}
