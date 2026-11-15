using BuyoraDataAccess.Data;
using BuyoraModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuyoraDataAccess.DbInitializer
{
    public class DbInitilizer : IDbInitilizer
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbInitilizer(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task InitilizeAsync()
        {
            if(_db.Database.CanConnect())
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    await _db.Database.MigrateAsync();
                }
                else
                {
                    if (!_db.Roles.Any())
                    {
                        _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
                        _roleManager.CreateAsync(new IdentityRole("Employee")).GetAwaiter().GetResult();
                        _roleManager.CreateAsync(new IdentityRole("Customer")).GetAwaiter().GetResult();
                    }
                }
                var user = await _userManager.FindByEmailAsync("admin@gmail.com");
                if(user == null)
                {
                    user = new ApplicationUser()
                    {
                        UserName = "admin",
                        Email = "admin@gmail.com",
                        EmailConfirmed = true,
                        PhoneNumber = "01000000000",
                        Name = "Admin",
                        Address = "Egypt"
                    };
                    var res = await _userManager.CreateAsync(user, "Admin@123");
                    if(res.Succeeded == true)
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    
                }
            }
        }
    }
}
