using muatamer_camunda_poc.HostedServices;

namespace muatamer_camunda_poc.Context
{
    public static class DbInitializerInstaller
    {
        public static IServiceCollection AddDbInitializer(this IServiceCollection services)
        {
            services.AddScoped<DbSeed>();
            services.AddHostedService<DbInitializer>();
            return services;
        }
    }
}