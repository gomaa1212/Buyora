using BuyoraDataAccess.Data;
using BuyoraModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;


namespace BuyoraDataAccess.DbInitializer
{
    public class DbInitilizer : IDbInitilizer
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public DbInitilizer(
            AppDbContext db,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task InitilizeAsync()
        {
            await _db.Database.MigrateAsync();

            foreach (var roleName in new[] { "Admin", "Employee", "Customer" })
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!roleResult.Succeeded)
                        throw new InvalidOperationException($"Could not create the {roleName} role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
            }

            var adminEmail = _configuration["Admin:Email"];
            var adminPassword = _configuration["Admin:Password"];
            if (string.IsNullOrWhiteSpace(adminEmail) && string.IsNullOrWhiteSpace(adminPassword))
                return;

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
                throw new InvalidOperationException("Set both Admin:Email and Admin:Password, or leave both unset.");

            var admin = await _userManager.FindByEmailAsync(adminEmail);
            if (admin is null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Name = "Buyora Administrator",
                    Address = string.Empty
                };

                var createResult = await _userManager.CreateAsync(admin, adminPassword);
                if (!createResult.Succeeded)
                    throw new InvalidOperationException($"Could not create the configured administrator: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }

            if (!await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                var addRoleResult = await _userManager.AddToRoleAsync(admin, "Admin");
                if (!addRoleResult.Succeeded)
                    throw new InvalidOperationException($"Could not grant the Admin role: {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");
            }
        }
    }
}
