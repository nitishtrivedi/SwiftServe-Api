using System.ComponentModel.DataAnnotations;

namespace SwiftServe_API.Models
{
    public class User : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; } // SuperAdmin, Admin, Customer, DeliveryPartner

        public int? RestaurantId { get; set; }
    }
}
