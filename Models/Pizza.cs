using System.ComponentModel.DataAnnotations;

namespace PizzaHub.Models
{
    public class Pizza
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        public string ImageFileName { get; set; } = "no-image.jpg"; // default image
    }
}