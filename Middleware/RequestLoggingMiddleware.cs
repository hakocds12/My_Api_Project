// Middleware/RequestLoggingMiddleware.cs
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics; // Needed for Stopwatch
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // Using the standard logging framework

namespace UserManagementAPI.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger; // Inject the built-in logger
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Log BEFORE the request goes down the pipeline
            _logger.LogInformation($"---> Request: {context.Request.Method} {context.Request.Path}");

            // Keep the original response body stream so we can read it later (if needed)
            // and then restore it so the response can be sent to the client.
            // This part is more complex if you need to log the RESPONSE BODY.
            // For this activity, let's stick to logging method, path, and status code which is simpler.

            try
            {
                // Call the next middleware in the pipeline
                await _next(context);
            }
            finally // finally ensures this code runs even if an exception occurs later in the pipeline
            {
                stopwatch.Stop();

                // Log AFTER the request has been processed
                // Use context.Response.StatusCode to get the final status code
                _logger.LogInformation($"<--- Response: {context.Request.Method} {context.Request.Path} responded {context.Response.StatusCode} in {stopwatch.ElapsedMilliseconds}ms");
            }
        }
    }

    // Optional: Extension method to make adding the middleware easier in Program.cs
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}