using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace API.Middleware
{
    public class AuthMiddleware
    {

        private readonly RequestDelegate _next;
        private const string ApiKeyHeaderName = "X-API-KEY";
        private const string ValidApiKey = "my-secret-key";

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the request contains the required header
            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("API Key was missing.");
                return;
            }

            // Validate the API key
            if (!ValidApiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Invalid API Key.");
                return;
            }

            await _next(context);
        }
    }
}