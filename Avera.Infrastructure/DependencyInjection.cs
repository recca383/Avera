using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Infrastructure.Authentication;
using Avera.Infrastructure.Database.Application;
using Avera.Infrastructure.Database.Identity;
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
            services.AddServices()
            .AddDatabase(configuration)
            .AddHealthChecks(configuration)
            .AddAuthenticationInternal(configuration)
            .AddAuthorizationInternal();

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
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
             options.UseSqlServer(applicationConnectionString, sqlServerOptionsAction: sqlOptions =>
             {
                 sqlOptions.EnableRetryOnFailure(
                    maxRetryCount:3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd:null
                 );
             }));

            services.AddDbContext<IApplicationIdentityDbContext, ApplicationIdentityDbContext>(options =>
             options.UseSqlServer(identityConnectionString, sqlServerOptionsAction: sqlOptions =>
             {
                 sqlOptions.EnableRetryOnFailure(
                    maxRetryCount:3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd:null
                 );
             }));

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
