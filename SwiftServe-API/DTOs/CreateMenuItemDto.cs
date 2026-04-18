namespace SwiftServe_API.DTOs
{
    public class CreateMenuItemDto
    {
        public int RestaurantId { get; set; }
        public int CategoryId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public decimal Price { get; set; }

        public IFormFile File { get; set; }
    }

    public class UpdateMenuItemDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }

        public IFormFile File { get; set; } // optional
    }

    public class MenuItemResponseDto
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public int CategoryId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public decimal Price { get; set; }
        public string ImageUrl { get; set; }

        public bool IsAvailable { get; set; }
    }
}
