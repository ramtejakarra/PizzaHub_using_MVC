using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaHub.Data;
using PizzaHub.Models;
using System.IO;

namespace PizzaHub.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Admin/ManagePizzas
        // Show all pizzas with edit/delete buttons
        public IActionResult ManagePizzas()
        {
            var pizzas = _context.Pizzas.ToList();
            return View(pizzas);
        }

        // GET: /Admin/CreatePizza
        // Show form to add new pizza
        public IActionResult CreatePizza()
        {
            return View();
        }

        // POST: /Admin/CreatePizza
        // Save new pizza to database
        [HttpPost]
        public async Task<IActionResult> CreatePizza(Pizza pizza, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var path = Path.Combine(_env.WebRootPath, "images", "pizzas", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    pizza.ImageFileName = fileName;
                }
                else
                {
                    pizza.ImageFileName = "no-image.jpg";
                }

                _context.Pizzas.Add(pizza);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Pizza added successfully.";
                return RedirectToAction("ManagePizzas");
            }

            return View(pizza);
        }

        // GET: /Admin/EditPizza/5
        // Show form to edit pizza
        public async Task<IActionResult> EditPizza(int id)
        {
            var pizza = await _context.Pizzas.FindAsync(id);
            if (pizza == null)
            {
                TempData["Error"] = "Pizza not found.";
                return RedirectToAction("ManagePizzas");
            }
            return View(pizza);
        }

        // POST: /Admin/EditPizza/5
        // Save edited pizza
        [HttpPost]
        public async Task<IActionResult> EditPizza(int id, Pizza pizza, IFormFile? imageFile)
        {
            if (id != pizza.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var oldPizza = await _context.Pizzas.FindAsync(id);
                if (oldPizza == null)
                {
                    return NotFound();
                }

                // Handle image upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Delete old image
                    var oldImagePath = Path.Combine(_env.WebRootPath, "images", "pizzas", oldPizza.ImageFileName);
                    if (System.IO.File.Exists(oldImagePath) && oldPizza.ImageFileName != "no-image.jpg")
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                    // Save new image
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var path = Path.Combine(_env.WebRootPath, "images", "pizzas", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    pizza.ImageFileName = fileName;
                }
                else
                {
                    pizza.ImageFileName = oldPizza.ImageFileName;
                }

                // Update values
                oldPizza.Name = pizza.Name;
                oldPizza.Description = pizza.Description;
                oldPizza.Price = pizza.Price;
                oldPizza.ImageFileName = pizza.ImageFileName;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Pizza updated.";
                return RedirectToAction("ManagePizzas");
            }

            return View(pizza);
        }

        // POST: /Admin/DeletePizza/5
        // Delete a pizza
        [HttpPost]
        public async Task<IActionResult> DeletePizza(int id)
        {
            var pizza = await _context.Pizzas.FindAsync(id);
            if (pizza == null)
            {
                TempData["Error"] = "Pizza not found.";
                return RedirectToAction("ManagePizzas");
            }

            // Delete image file
            var imagePath = Path.Combine(_env.WebRootPath, "images", "pizzas", pizza.ImageFileName);
            if (System.IO.File.Exists(imagePath) && pizza.ImageFileName != "no-image.jpg")
            {
                System.IO.File.Delete(imagePath);
            }

            _context.Pizzas.Remove(pizza);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Pizza deleted.";
            return RedirectToAction("ManagePizzas");
        }

        // GET: /Admin/AllOrders
        // View all orders from all users
        public IActionResult AllOrders()
        {
            var orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .ThenInclude(i => i.Pizza)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }
    }
}