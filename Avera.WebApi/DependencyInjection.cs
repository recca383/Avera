using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.RateLimiting;

namespace Avera.WebApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebApi(this IServiceCollection services)
        {
            services.AddEndpoints(Assembly.GetExecutingAssembly());
            services.AddOpenApi(options =>
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>()
            );
            services.AddAntiforgery();
            services.AddRateLimiter(options =>
            {
                options.AddPolicy("resend-verification", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));
            });

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            services.AddHttpClient();

            services.AddSignalR();

            return services;
        }
    }
}
