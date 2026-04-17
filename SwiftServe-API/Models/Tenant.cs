namespace SwiftServe_API.Models
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; }

        public int OwnerId { get; set; } // Admin user
    }
}
