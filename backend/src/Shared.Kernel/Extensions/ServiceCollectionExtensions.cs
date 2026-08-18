using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Kernel.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStandardizedValidationErrors(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .SelectMany(e => e.Value!.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    var message = string.Join(" | ", errors);
                    var response = new ErrorResponse(message, StatusCodes.Status400BadRequest);

                    return new BadRequestObjectResult(response);
                };
            });

            return services;
        }
    }
}
