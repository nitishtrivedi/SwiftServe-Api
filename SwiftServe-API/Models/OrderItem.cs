using System.ComponentModel.DataAnnotations;

namespace SwiftServe_API.Models
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }

        public int MenuItemId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; } // snapshot price
    }
}
