using System.ComponentModel.DataAnnotations;
using System.Data;

namespace PizzaHub.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public virtual ApplicationUser User { get; set; } = null;

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        public virtual List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
