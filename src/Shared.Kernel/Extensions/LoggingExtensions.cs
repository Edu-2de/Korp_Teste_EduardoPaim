using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Shared.Kernel.Extensions
{
    public static class LoggingExtensions
    {
        public static void UseSharedSerilog(this WebApplicationBuilder builder, string serviceName)
        {
            builder.Host.UseSerilog((context, services, configuration) =>
            {
                var minimumLevel = LogEventLevel.Information;

                configuration
                    .MinimumLevel.Is(minimumLevel)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Service", serviceName)
                    .WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] ({Service}) {Message:lj} {Properties:j}{NewLine}{Exception}");
            });
        }
    }
}
