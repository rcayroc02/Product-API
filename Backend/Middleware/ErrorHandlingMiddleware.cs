using System.Net;
using System.Text.Json;

namespace TuProyecto.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no controlado: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, message) = exception switch
            {
                KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message),
                ArgumentException => (HttpStatusCode.BadRequest, exception.Message),
                InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "No autorizado"),
                _ => (HttpStatusCode.InternalServerError, "Ocurrió un error interno en el servidor")
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                status = (int)statusCode,
                message,
                timestamp = DateTime.UtcNow
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}