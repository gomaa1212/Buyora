using Buyora.SharedRepository;
using Buyora.UnitOfWorks;

namespace Buyora.DependencyInjection
{
    public static class UnitOfWorkDependencyInjection
    {
        public static IServiceCollection AddUnitOFWorkDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            return services;
        }
    }
}
