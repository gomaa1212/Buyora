
using BuyoraDataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace Buyora.DependencyInjection
{
    public static class GeneralRegisterDependencyInjection
    {
        public static IServiceCollection AddGeneralRegisterDependencyInjection(this IServiceCollection services,IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Database connection is missing. Configure ConnectionStrings:DefaultConnection.");

            services.AddDbContext<AppDbContext>(x => x.UseSqlServer(connectionString));

            services.AddAutoMapper(
                cfg => { },
                typeof(Program).Assembly
            );

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(300);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            return services;
        }
    }
}
