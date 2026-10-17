using System.Net;
using System.Text.Json;

namespace CommerceApi.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción capturada por Middleware Global.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                ArgumentException or FormatException => HttpStatusCode.BadRequest,
                KeyNotFoundException => HttpStatusCode.NotFound,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                statusCode = context.Response.StatusCode,
                message = exception.Message
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}