namespace SwiftServe_API.Models
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public int? TenantId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
