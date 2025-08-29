using Microsoft.EntityFrameworkCore;
using PizzaHub.Data;
using PizzaHub.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ======== 1. Database Context (Entity Framework) ========
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ======== 2. Identity with Custom User & Roles ========
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // No email confirmation needed
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false; // Optional: disable special char requirement
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>() // Enable roles (e.g., Admin)
.AddEntityFrameworkStores<ApplicationDbContext>(); // Store data in your DB

// ======== 3. Session Support (for cart item count in navbar) ========
builder.Services.AddDistributedMemoryCache(); // Stores session in memory
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session expires after 30 min
    options.Cookie.HttpOnly = true;  // Helps prevent XSS
    options.Cookie.IsEssential = true; // Required even if user blocks cookies
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // For wwwroot (images, CSS, JS)

app.UseRouting();

app.UseAuthentication(); // Check if user is logged in
app.UseAuthorization();  // Check roles/permissions

// ======== IMPORTANT: Session middleware must come AFTER UseAuthentication ========
app.UseSession();

// ======== Seed Admin Role and User on Startup ========
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    // Apply any pending migrations (creates DB/tables if not exist)
    context.Database.Migrate();

    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Seed Admin role and user
    await SeedAdminUserAsync(userManager, roleManager);
}

// ======== Map Razor Pages (for Identity: Login, Register, etc.) ========
app.MapRazorPages();

// ======== Map MVC Routes (Home, Menu, Cart, etc.) ========
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ======== Start the app ========
await app.RunAsync();

// ======== Helper Method: Seed Admin User & Role ========
static async Task SeedAdminUserAsync(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager)
{
    // Create "Admin" role if it doesn't exist
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Check if admin user already exists
    var adminEmail = "admin@pizzahub.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        // Create new admin user
        var user = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "Admin User" // Custom property
        };

        var result = await userManager.CreateAsync(user, "Admin@123");

        if (result.Succeeded)
        {
            // Add the user to the Admin role
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}