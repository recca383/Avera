using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using System.Reflection;
using System.Runtime.Serialization;

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

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            services.AddHttpClient();

            return services;
        }
    }
}
