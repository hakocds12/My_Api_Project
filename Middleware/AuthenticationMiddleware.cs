// Middleware/AuthenticationMiddleware.cs
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration; // Needed to read configuration (like API key)

namespace UserManagementAPI.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthenticationMiddleware> _logger;
        private readonly IConfiguration _configuration; // Inject Configuration

        // Define the header name where the token is expected
        private const string APIKeyHeaderName = "X-API-Key"; // Common practice for simple API keys

        public AuthenticationMiddleware(RequestDelegate next,
                                      ILogger<AuthenticationMiddleware> logger,
                                      IConfiguration configuration) // Inject Configuration
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation($"AuthenticationMiddleware: Checking for {APIKeyHeaderName} header.");

            // Get the API key from the request header
            if (!context.Request.Headers.TryGetValue(APIKeyHeaderName, out var receivedApiKey))
            {
                // If the header is missing, return 401 Unauthorized
                _logger.LogWarning($"AuthenticationMiddleware: Missing {APIKeyHeaderName} header.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized; // Use StatusCodes class for clarity
                await context.Response.WriteAsync("API Key header was not provided."); // Optional response body
                return; // Stop processing the pipeline
            }

            // Get the expected API key from configuration
            // Make sure you have a setting like "Authentication:ApiKey" in your appsettings.json
            var expectedApiKey = _configuration["Authentication:ApiKey"];

            // Check if the received API key matches the expected one
            // Use StringComparer.Ordinal to avoid timing attacks (safer comparison)
            if (!StringComparer.Ordinal.Equals(receivedApiKey, expectedApiKey))
            {
                // If the API keys don't match, return 401 Unauthorized
                 _logger.LogWarning($"AuthenticationMiddleware: Invalid {APIKeyHeaderName} provided.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid API Key."); // Optional response body
                return; // Stop processing the pipeline
            }

            // If the API key is valid, allow the request to proceed
            _logger.LogInformation($"AuthenticationMiddleware: Valid {APIKeyHeaderName} provided. Request proceeding.");
            await _next(context);
        }
    }

    // Optional: Extension method
    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}