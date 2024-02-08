using muatamer_camunda_poc.Context;

namespace muatamer_camunda_poc.HostedServices
{
    public class DbInitializer : IHostedService
    {
        private readonly IServiceProvider serviceProvider;

        public DbInitializer(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();
            await scope.ServiceProvider.GetService<DbSeed>().SeedData();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}