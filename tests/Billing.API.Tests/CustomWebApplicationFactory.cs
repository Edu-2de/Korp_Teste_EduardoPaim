using Billing.API.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Testcontainers.PostgreSql;
using Xunit;

namespace Billing.API.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {

        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("billing_test_db")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<BillingDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<BillingDbContext>(options =>
                    options.UseNpgsql(_dbContainer.GetConnectionString()));
            });
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();

            var options = new DbContextOptionsBuilder<BillingDbContext>()
                .UseNpgsql(_dbContainer.GetConnectionString())
                .Options;

            await using var context = new BillingDbContext(options);
            await context.Database.MigrateAsync();
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await _dbContainer.DisposeAsync();
        }
    }
}
