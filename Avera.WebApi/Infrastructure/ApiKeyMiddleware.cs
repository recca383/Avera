using System.Security.Cryptography;

namespace Avera.WebApi.Infrastructure
{
    internal sealed class ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        private const string HeaderName = "X-Api-Key";

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/scalar") || 
                context.Request.Path.StartsWithSegments("/openapi"))
            {
                await next(context);
                return;
            }

            string? expectedKey = configuration.GetValue<string>("External-Api-Key");
            string? providedKey = context.Request.Headers[HeaderName].FirstOrDefault();

            Console.WriteLine($"Expected Key: {expectedKey}");
            Console.WriteLine($"Provided Key: {providedKey}");
            if (string.IsNullOrEmpty(expectedKey) || 
                string.IsNullOrEmpty(providedKey) ||
                !CryptographicOperations.FixedTimeEquals(
                    System.Text.Encoding.UTF8.GetBytes(providedKey),
                    System.Text.Encoding.UTF8.GetBytes(expectedKey)))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { title = "Invalid API Key" });
                return;
            }

            await next(context);
        }
    }   

}