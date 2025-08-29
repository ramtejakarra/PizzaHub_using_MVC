using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaHub.Data;
using PizzaHub.Models;

namespace PizzaHub.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Order/Checkout
        // Review cart before placing order
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                TempData["Error"] = "Please log in to place an order.";
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _context.CartItems
                .Include(c => c.Pizza)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            ViewBag.Total = cartItems.Sum(i => i.Pizza.Price * i.Quantity);
            return View(cartItems);
        }

        // POST: /Order/PlaceOrder
        // Creates the order from cart
        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                TempData["Error"] = "Please log in first.";
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _context.CartItems
                .Include(c => c.Pizza)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Cannot place empty order.";
                return RedirectToAction("Index", "Cart");
            }

            // Create new order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(i => i.Pizza.Price * i.Quantity),
                Items = cartItems.Select(i => new OrderItem
                {
                    PizzaId = i.PizzaId,
                    Quantity = i.Quantity,
                    PriceAtTime = i.Pizza.Price
                }).ToList()
            };

            // Save order
            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems); // Clear cart
            await _context.SaveChangesAsync();

            // Update session cart count
            HttpContext.Session.SetInt32("CartItems", 0);

            TempData["Success"] = "Your order has been placed successfully!";
            return RedirectToAction("OrderPlaced", new { id = order.Id });
        }

        // GET: /Order/OrderPlaced/{id}
        // Show success page after order
        public async Task<IActionResult> OrderPlaced(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Pizza)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("History");
            }

            return View(order);
        }

        // GET: /Order/History
        // Show all past orders for user
        public async Task<IActionResult> History()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return RedirectToAction("Login", "Account");

            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Pizza)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }
    }
}