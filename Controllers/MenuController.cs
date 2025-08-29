using Microsoft.AspNetCore.Mvc;
using PizzaHub.Data;
using PizzaHub.Models;

namespace PizzaHub.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Menu
        public IActionResult Index()
        {
            var pizzas = _context.Pizzas.ToList();

            if (!pizzas.Any())
            {
                TempData["Error"] = "No pizzas available at the moment.";
            }

            return View(pizzas);
        }
    }
}