namespace Shared.Kernel
{
    public class ErrorResponse(string message, int statusCode, string? detail = null)
    {
        public string Message { get; set; } = message;
        public string? Detail { get; set; } = detail;
        public int StatusCode { get; set; } = statusCode;
    }
}
