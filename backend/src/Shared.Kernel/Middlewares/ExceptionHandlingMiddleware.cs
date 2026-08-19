using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;

namespace Shared.Kernel.Middlewares
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, message) = exception switch
            {
                KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message),
                InvalidOperationException => (HttpStatusCode.Conflict, exception.Message),
                ArgumentException => (HttpStatusCode.BadRequest, exception.Message),

                DbUpdateConcurrencyException =>
                    (HttpStatusCode.Conflict, "Este registro foi alterado por outra operação simultânea. Tente novamente."),

                HttpRequestException =>
                    (HttpStatusCode.ServiceUnavailable, "Um serviço do sistema está indisponível no momento. Tente novamente em instantes."),
                TaskCanceledException =>
                    (HttpStatusCode.ServiceUnavailable, "Um serviço do sistema demorou muito para responder. Tente novamente em instantes."),
                _ => (HttpStatusCode.InternalServerError, "Ocorreu um erro inesperado. Tente novamente."),
            };

            var response = new ErrorResponse(message, (int)statusCode);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
        }
    }
}
