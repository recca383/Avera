using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.ML;
using Avera.Application.Abstractions.Storage;
using Avera.Infrastructure.Authentication;
using Avera.Infrastructure.Database.Application;
using Avera.Infrastructure.Database.Identity;
using Avera.Infrastructure.Identity.Roles;
using Avera.Infrastructure.Identity.Users;
using Avera.Infrastructure.ML;
using Avera.Infrastructure.Storage;
using Avera.Infrastructure.Time;
using Infrastructure.DomainEvents;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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
            
            services.AddDbContext<IdentityDbContext>(options =>
             options.UseNpgsql(identityConnectionString));

            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;

                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();


            return services;
        }

        private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        private static IServiceCollection AddAuthenticationInternal(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    // {
                    //     ValidateIssuer = true,
                    //     ValidateAudience = true,
                    //     ValidateLifetime = true,
                    //     ValidateIssuerSigningKey = true,
                    //     ValidIssuer = configuration["Jwt:Issuer"],
                    //     ValidAudience = configuration["Jwt:Audience"],
                    //     IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured.")))
                    // };
                });

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<JwtProvider>();

            services.AddHttpContextAccessor();
            services.AddScoped<IUserContext, UserContext>();

            return services;
        }

        private static IServiceCollection AddAuthorizationInternal(this IServiceCollection services)
        { 
            services.AddAuthorization();
            return services;
        }
    }


}
