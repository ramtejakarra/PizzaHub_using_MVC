using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaHub.Data;
using PizzaHub.Models;

namespace PizzaHub.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Cart
        public IActionResult Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                TempData["Error"] = "Please log in to view your cart.";
                return RedirectToAction("Login", "Account");
            }

            var cartItems = _context.CartItems
                .Include(c => c.Pizza)
                .Where(c => c.UserId == userId)
                .ToList();

            ViewBag.Total = cartItems.Sum(item => item.Pizza.Price * item.Quantity);

            return View(cartItems);
        }

        // POST: /Cart/Add/5
        [HttpPost]
        public async Task<IActionResult> Add(int id, int quantity = 1)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                TempData["Error"] = "Please log in first.";
                return RedirectToAction("Login", "Account");
            }

            var pizza = await _context.Pizzas.FindAsync(id);
            if (pizza == null)
            {
                TempData["Error"] = "Pizza not found.";
                return RedirectToAction("Index", "Menu");
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.PizzaId == id && c.UserId == userId);

            if (cartItem == null)
            {
                cartItem = new CartItem { UserId = userId, PizzaId = id, Quantity = 0 };
                _context.CartItems.Add(cartItem);
            }

            cartItem.Quantity += quantity;
            await _context.SaveChangesAsync();

            UpdateCartCount();
            TempData["Success"] = $"{pizza.Name} added to cart!";
            return RedirectToAction("Index", "Menu");
        }

        private void UpdateCartCount()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var count = _context.CartItems.Where(c => c.UserId == userId).Sum(c => c.Quantity);
            HttpContext.Session.SetInt32("CartItems", count);
        }
    }
}
