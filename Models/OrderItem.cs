namespace PizzaHub.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int PizzaId { get; set; }
        public virtual Pizza Pizza { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal PriceAtTime { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;

    }
}
