using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.ML;
using Avera.Application.Abstractions.Storage;
using Avera.Infrastructure.Authentication;
using Avera.Infrastructure.Database.Application;
using Avera.Infrastructure.Database.Identity;
using Avera.Infrastructure.ML;
using Avera.Infrastructure.Storage;
using Avera.Infrastructure.Time;
using Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;

namespace Avera.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) =>
            services.AddServices(configuration)
            .AddDatabase(configuration)
            .AddHealthChecks(configuration)
            .AddAuthenticationInternal(configuration)
            .AddAuthorizationInternal();

        private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IMLService, MLService>();    
            services.AddScoped<IBlobStorageService, AzureBlobStorageService>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();

            return services;
        }

        private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            System.Console.WriteLine("Loading Configuration...");
            string? applicationConnectionString = configuration["ApplicationDbConnectionString"];
            string? identityConnectionString = configuration["ApplicationIdentityDbConnectionString"];

            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
             options.UseNpgsql(applicationConnectionString));

            services.AddDbContext<IApplicationIdentityDbContext, ApplicationIdentityDbContext>(options =>
             options.UseNpgsql(identityConnectionString));

            return services;
        }

        private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        private static IServiceCollection AddAuthenticationInternal(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IUserContext, UserContext>();

            return services;
        }

        private static IServiceCollection AddAuthorizationInternal(this IServiceCollection services)
        { 
            return services;
        }
    }


}
