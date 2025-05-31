// Middleware/ErrorHandlerMiddleware.cs
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting; // <-- Make sure this using directive is here

namespace UserManagementAPI.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        // --- Add this field ---
        private readonly IWebHostEnvironment _env;
        // --- End Add ---

        // --- Modify the constructor to accept IWebHostEnvironment ---
        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env; // --- Assign the injected environment ---
        }
        // --- End Modify Constructor ---


               public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred while processing the request.");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // --- Change this line ---
                // var response = new { message = "An unexpected error occurred." }; // <-- Remove initial assignment here
                // --- To this line ---
                object response; // Declare 'response' as object
                // --- End Change ---


                if (_env.IsDevelopment())
                {
                    // In Development, include exception details
                    // --- This assignment is now allowed because 'response' is 'object' ---
                    response = new { message = ex.Message, details = ex.StackTrace };
                    // --- End Assignment ---
                }
                else
                {
                    // In Production, return a generic message
                    // --- This assignment is now allowed because 'response' is 'object' ---
                    response = new { message = "An unexpected error occurred." };
                    // --- End Assignment ---
                }


                var jsonResponse = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }

    // Optional: Extension method remains the same
    public static class ErrorHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}