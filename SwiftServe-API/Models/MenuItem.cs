using System.ComponentModel.DataAnnotations;

namespace SwiftServe_API.Models
{
    public class MenuItem : BaseEntity
    {
        public int RestaurantId { get; set; }
        public int CategoryId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
