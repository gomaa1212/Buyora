using BuyoraDataAccess.Data;
using BuyoraModels;
using Microsoft.AspNetCore.Identity;

namespace Buyora.DependencyInjection
{
    public static class IdentityDependencyInjection
    {
        public static IServiceCollection AddIdentityDependencyInjection(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
            {
                //password setting
                opt.Password.RequireLowercase = true;
                opt.Password.RequireDigit = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequiredLength = 6;
                opt.Password.RequireNonAlphanumeric = false;

                //Lock out setting
                opt.Lockout.MaxFailedAccessAttempts = 6;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                //user setting
                opt.User.RequireUniqueEmail = true;


                //signin
                opt.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
