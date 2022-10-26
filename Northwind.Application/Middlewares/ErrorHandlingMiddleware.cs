﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Northwind.Application.Exceptions;
using Northwind.Application.Extensions;
using System.Net;
using System.Text.Json;

namespace Northwind.Application.Middlewares
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
            var code = exception switch
            {
                TaskCanceledException => HttpStatusCode.Accepted,
                PaginationException => HttpStatusCode.BadRequest,
                _=> HttpStatusCode.InternalServerError
            };

            var message = exception switch
            {
                TaskCanceledException => "Operation was cancelled.",
                PaginationException => exception.Message,
                _ => "An error occurred while processing the request."
            };

            var result = JsonSerializer.Serialize(new { error = message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            await context.Response.WriteAsync(result);
        }
    }
}
