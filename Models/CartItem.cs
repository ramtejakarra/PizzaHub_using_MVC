using System.ComponentModel.DataAnnotations;

namespace PizzaHub.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public int PizzaId { get; set; }

        public virtual Pizza Pizza { get; set; } = null!; // ← Removed 'required'

        public int Quantity { get; set; } = 1;

        public string? UserId { get; set; }
    }
}