using Microsoft.AspNetCore.Builder;
using Shared.Kernel.Middlewares;

namespace Shared.Kernel.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
