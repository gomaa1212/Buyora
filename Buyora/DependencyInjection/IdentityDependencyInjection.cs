using BuyoraDataAccess.Data;
using BuyoraModels;
using Microsoft.AspNetCore.Identity;

namespace Buyora.DependencyInjection
{
    public static class IdentityDependencyInjection
    {
        public static IServiceCollection AddIdentityDependencyInjection(this IServiceCollection services,IConfiguration configuration)
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

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            services.AddAuthentication()
                            .AddGoogle(options =>
                            {
                                options.ClientId = configuration["Authentication:Google:ClientId"]
                                    ?? throw new InvalidOperationException("Google ClientId is missing");
                                options.ClientSecret = configuration["Authentication:Google:ClientSecret"]
                                    ?? throw new InvalidOperationException("Google ClientSecret is missing");
                            });
            return services;
        }
    }
}
