namespace SwiftServe_API.DTOs
{
    public class CreateCategoryDto
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
