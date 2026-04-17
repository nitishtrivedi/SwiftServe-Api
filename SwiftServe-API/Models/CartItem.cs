namespace SwiftServe_API.Models
{
    public class CartItem : BaseEntity
    {
        public int CartId { get; set; }

        public int MenuItemId { get; set; }

        public int Quantity { get; set; }
    }
}
