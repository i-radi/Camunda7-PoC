using Microsoft.Extensions.DependencyInjection;

namespace muatamer_camunda_poc.Init
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