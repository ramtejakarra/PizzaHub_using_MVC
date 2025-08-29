using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PizzaHub.Models;

namespace PizzaHub.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Pizza> Pizzas { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Table names
            builder.Entity<Pizza>().ToTable("Pizzas");
            builder.Entity<Order>().ToTable("Orders");
            builder.Entity<OrderItem>().ToTable("OrderItems");
            builder.Entity<CartItem>().ToTable("CartItems");

            // Configure precision for decimals (avoid warnings & truncation issues)
            builder.Entity<Pizza>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            builder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            builder.Entity<OrderItem>()
                .Property(oi => oi.PriceAtTime)
                .HasPrecision(18, 2);

            
            // Seed sample Pizzas
            builder.Entity<Pizza>().HasData(
                new Pizza
                {
                    Id = 1,
                    Name = "Margherita",
                    Description = "Classic delight with 100% real mozzarella cheese",
                    Price = 399.00m,
                    ImageFileName = "margherita.jpg"
                },
                new Pizza
                {
                    Id = 2,
                    Name = "Pepperoni",
                    Description = "A classic American taste! Relish the delectable flavor of Pepperoni, topped with extra cheese",
                    Price = 297.00m,
                    ImageFileName = "pepperoni.jpg"
                },
                new Pizza
                {
                    Id = 3,
                    Name = "Veggie Supreme",
                    Description = "A colorful mix of fresh veggies including bell peppers, olives, onions, and mushrooms",
                    Price = 258.00m,
                    ImageFileName = "veggie_supreme.jpg"
                },
                new Pizza
                {
                    Id = 4,
                    Name = "Hawaiian",
                    Description = "A tropical treat with ham and pineapple",
                    Price = 366.00m,
                    ImageFileName = "hawaiian.jpg"
                },
                new Pizza
                {
                    Id = 5,
                    Name = "Meat Lovers",
                    Description = "Loaded with pepperoni, sausage, ham, and bacon for true meat lovers",
                    Price = 397.00m,
                    ImageFileName = "meatlovers.jpg"
                }
            );
        }
    }
}
