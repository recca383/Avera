using Avera.Application.Abstractions.Services;
using Avera.Domain.Identity.Users;
using Avera.Infrastructure.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharedKernel;
using System.Web;

namespace Avera.WebApi.Endpoints
{
    public class TestEmail(IConfiguration configuration) : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("test", async (
                IEmailService service,
                UserManager<User> _userManager,
                IDateTimeProvider dateTime,
                [FromServices] IOptions<AppOptions> appOptions,
                CancellationToken cancellationToken) =>
            {
                var testEmail = "crusitwincel@gmail.com";
                var user = _userManager.FindByEmailAsync(testEmail).Result;

                var verificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user!);

                var newEmail = "patrickfernandez.dev@gmail.com";
                var changeEmailToken = await _userManager.GenerateChangeEmailTokenAsync(user!, newEmail);

                var apiUrl = appOptions.Value.PublicBaseUrl.TrimEnd('/');


                var verificationUrl =
                    $"{apiUrl}/auth/verify-email" +
                    $"?userId={Uri.EscapeDataString(user.Id.ToString())}" +
                    $"&token={Uri.EscapeDataString(verificationToken)}";
 
                var date = DateOnly.FromDateTime(dateTime.PhilippineNow);

                var time = TimeOnly.FromDateTime(dateTime.PhilippineNow);

                // return configuration["ApplicationDbConnectionString"] + "\n\n" + configuration["ApplicationIdentityDbConnectionString"];

                await service.SendEmailVerificationAsync(user.Email, user.FirstName,
                verificationUrl, cancellationToken);

                //await service.SendEmailChangeVerificationAsync(user.Email, user.FirstName,
                //$"{changeEmailToken}", cancellationToken);

                //await service.SendEmailNotificationToNewEmail(user.Email, user.FirstName,
                //date, time, appUrl, cancellationToken);

                //await service.SendEmailVerified(user.Email, user.FirstName,
                //appUrl, cancellationToken);

                //await service.SendEmailNotificationToOldEmail(user.Email, user.FirstName, newEmail, date, time);


            });
            
        }
    }
}