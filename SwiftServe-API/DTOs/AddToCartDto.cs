namespace SwiftServe_API.DTOs
{
    public class AddToCartDto
    {
        public int RestaurantId { get; set; }
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
    }
}
