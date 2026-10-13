namespace Buyora.DependencyInjection
{
    public static class LocalizationDependencyInjection
    {
        public static IServiceCollection AddLocalizationDependencyInjection(this IServiceCollection services)
        {
            services.AddLocalization();
            services.AddControllersWithViews()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();
            return services;
        }
    }
}
