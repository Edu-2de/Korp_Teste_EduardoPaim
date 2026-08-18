using CorrelationId;
using CorrelationId.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Kernel.Extensions
{
    public static class CorrelationIdExtensions
    {
        public static IServiceCollection AddSharedCorrelationId(this IServiceCollection services)
        {
            services.AddDefaultCorrelationId(options =>
            {
                options.RequestHeader = "X-Correlation-Id";
                options.ResponseHeader = "X-Correlation-Id";
                options.AddToLoggingScope = true;
            });

            return services;
        }

        public static IApplicationBuilder UseSharedCorrelationId(this IApplicationBuilder app)
        {
            return app.UseCorrelationId();
        }
    }
}
