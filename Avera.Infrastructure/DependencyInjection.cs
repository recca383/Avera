using Avera.Application.Abstractions.Databases;
using Avera.Infrastructure.Database.Application;
using Avera.Infrastructure.Database.Identity;
using Avera.Infrastructure.Time;
using Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;
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
            string? applicationConnectionString = configuration.GetConnectionString("ApplicationDB");
            string? identityConnectionString = configuration.GetConnectionString("IdentityDB");

            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options => options.UseSqlServer(applicationConnectionString));
            services.AddDbContext<IApplicationIdentityDbContext, ApplicationIdentityDbContext>(options => options.UseSqlServer(identityConnectionString));

            return services;
        }

        private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        private static IServiceCollection AddAuthenticationInternal(this IServiceCollection services, IConfiguration configuration)
        {


            return services;
        }

        private static IServiceCollection AddAuthorizationInternal(this IServiceCollection services)
        { 
            return services;
        }
    }


}
