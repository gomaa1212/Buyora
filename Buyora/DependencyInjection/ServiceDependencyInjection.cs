using Buyora.Models.Services.Implementations;
using Buyora.Models.Services.Interfaces;

namespace Buyora.DependencyInjection
{
    public static class ServiceDependencyInjection
    {
        public static IServiceCollection AddServiceDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            return services;
        }
    }
}
