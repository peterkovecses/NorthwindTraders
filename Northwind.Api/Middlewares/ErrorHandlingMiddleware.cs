﻿using Northwind.Application.Exceptions;
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
                _logger.LogError(ex, "An exception occured");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            (HttpStatusCode code, string message) = exception switch
            {
                OperationCanceledException => (HttpStatusCode.Accepted, "Operation was cancelled."),
                PropertyNotFoundException => (HttpStatusCode.BadRequest, exception.Message),
                PaginationException => (HttpStatusCode.BadRequest, exception.Message),
                ValueAboveMaxPageSizeException => (HttpStatusCode.BadRequest, exception.Message),
                ArgumentOutOfRangeException => (HttpStatusCode.BadRequest, exception.Message),
                ItemNotFoundException => (HttpStatusCode.NotFound, exception.Message),
                _ => (HttpStatusCode.InternalServerError, "An error occurred while processing the request.")
            };           

            var result = JsonSerializer.Serialize(new { error = message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            await context.Response.WriteAsync(result);
        }
    }
}
