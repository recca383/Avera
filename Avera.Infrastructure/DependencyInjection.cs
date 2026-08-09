using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.ML;
using Avera.Application.Abstractions.Services;
using Avera.Application.Abstractions.Storage;
using Avera.Domain.Identity.Roles;
using Avera.Domain.Identity.Users;
using Avera.Infrastructure.Authentication;
using Avera.Infrastructure.Authorization;
using Avera.Infrastructure.Database.Application;
using Avera.Infrastructure.Database.Identity;
using Avera.Infrastructure.ML;
using Avera.Infrastructure.Services;
using Avera.Infrastructure.Storage;
using Avera.Infrastructure.Time;
using Infrastructure.DomainEvents;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
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


            var client = new SmtpClient(configuration["SMTP:Host"], Convert.ToInt32(configuration["SMTP:Port"]))
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(configuration["SMTP:SenderEmail"], configuration["SMTP:Password"])
            };

            services.AddFluentEmail(configuration["SMTP:SenderEmail"], configuration["SMTP:SenderName"])
                .AddSmtpSender(client)
                .AddRazorRenderer();
                
            services.AddScoped<IEmailService, EmailService>();
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
                options.Password.RequireNonAlphanumeric = true;

                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(Convert.ToInt32(configuration["Identity:TokenExpiryInMinutes"]));
            });

            return services;
        }

        private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        private static IServiceCollection AddAuthenticationInternal(this IServiceCollection services, IConfiguration configuration)
        {
             var issuer = configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
            "       JWT issuer is not configured.");

            var audience = configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "JWT audience is not configured.");

            var secretKey = configuration["Jwt:Secret-Key"]
                ?? throw new InvalidOperationException(
                    "JWT signing key is not configured.");

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey));

            services
                .AddAuthentication(options => 
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,

                        ValidateAudience = true,
                        ValidAudience = audience,

                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = securityKey,

                        NameClaimType = ClaimTypes.NameIdentifier,
                        RoleClaimType = ClaimTypes.Role,

                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            Console.WriteLine("========== JWT MESSAGE RECEIVED ==========");

                            Console.WriteLine(
                                $"Authorization header: {context.Request.Headers.Authorization}");

                            Console.WriteLine(
                                $"Token exists: {!string.IsNullOrEmpty(context.Token)}");

                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine(
                                $"JWT FAILED: {context.Exception}");

                            return Task.CompletedTask;
                        },

                        OnTokenValidated = async context =>
                        {
                            var userManager =
                                context.HttpContext.RequestServices
                                    .GetRequiredService<UserManager<User>>();

                            var userId =
                                context.Principal!
                                    .FindFirstValue(ClaimTypes.NameIdentifier);

                            var securityStamp =
                                context.Principal!
                                    .FindFirstValue("SecurityStamp");

                            Console.WriteLine($"JWT User ID: {userId}");
                            Console.WriteLine($"JWT Security Stamp: {securityStamp}");

                            if (!Guid.TryParse(userId, out var parsedUserId))
                            {
                                context.Fail("Invalid user ID.");
                                return;
                            }

                            var user =
                                await userManager.FindByIdAsync(
                                    parsedUserId.ToString());

                            if (user is null)
                            {
                                context.Fail("User not found.");
                                return;
                            }

                            if (user.SecurityStamp != securityStamp)
                            {
                                context.Fail(
                                    "Token has been invalidated.");
                            }
                        }
                    };
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

            services.AddScoped<PermissionProvider>();

            services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

            services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
            return services;
        }
    }


}
