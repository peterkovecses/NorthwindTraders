using Northwind.Application.Exceptions;
using Northwind.Application.Extensions;
using System.Net;
using System.Text.Json;

namespace Northwind.Api.Middlewares
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

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            (HttpStatusCode code, string message) = exception switch
            {
                TaskCanceledException => (HttpStatusCode.Accepted, "Operation was cancelled."),
                PaginationException => (HttpStatusCode.BadRequest, exception.Message),
                ValueAboveMaxPageSizeException => (HttpStatusCode.BadRequest, exception.Message),
                ArgumentOutOfRangeException => (HttpStatusCode.BadRequest, exception.Message),
                ItemNotFoundException<int> => (HttpStatusCode.NotFound, exception.Message),
                ItemNotFoundException<string> => (HttpStatusCode.NotFound, exception.Message),
                ItemNotFoundException<(int, int)> => (HttpStatusCode.NotFound, exception.Message),
                _ => (HttpStatusCode.InternalServerError, "An error occurred while processing the request.")
            };           

            var result = JsonSerializer.Serialize(new { error = message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            await context.Response.WriteAsync(result);
        }
    }
}
