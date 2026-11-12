
using BuyoraDataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace Buyora.DependencyInjection
{
    public static class GeneralRegisterDependencyInjection
    {
        public static IServiceCollection AddGeneralRegisterDependencyInjection(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(x => x.UseSqlServer(configuration["Constr"]));

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
