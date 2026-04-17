using System.ComponentModel.DataAnnotations;

namespace SwiftServe_API.Models
{
    public class Restaurant : BaseEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public int OwnerId { get; set; }
    }
}
