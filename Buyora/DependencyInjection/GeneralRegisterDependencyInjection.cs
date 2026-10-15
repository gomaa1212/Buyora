
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
            return services;
        }
    }
}
