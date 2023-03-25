using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Models
{
    public class CreateRestaurantDto
    {
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [MaxLength(25)]
        public string Category { get; set; }
        [Required]
        public bool HasDelivery { get; set; }
        [Required]
        [EmailAddress]
        public string ContactEmail { get; set; }
        public string ContactNumber { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string PostalCode { get; set; }
    }
}
