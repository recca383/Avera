using Avera.Application.Abstractions.Services;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;

namespace Avera.WebApi.Endpoints
{
    public class TestEmail(IConfiguration configuration) : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("test", async (IEmailService service, UserManager<User> _userManager, CancellationToken cancellationToken) =>
            {
                var testEmail = "sirpatrick121402@gmail.com";
                var user = _userManager.FindByEmailAsync(testEmail).Result;

                var verificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user!);

                var newEmail = "patrickfernandez.dev@gmail.com";
                var changeEmailToken = await _userManager.GenerateChangeEmailTokenAsync(user!, newEmail);

                var appUrl = "avera://";


                var date = DateOnly.FromDateTime(DateTime.UtcNow);

                var time = TimeOnly.FromDateTime(DateTime.UtcNow);

                // return configuration["ApplicationDbConnectionString"] + "\n\n" + configuration["ApplicationIdentityDbConnectionString"];

                await service.SendEmailVerificationAsync(user.Email, user.FirstName,
                $"{verificationToken}", cancellationToken);

                await service.SendEmailChangeVerificationAsync(user.Email, user.FirstName,
                $"{changeEmailToken}", cancellationToken);

                await service.SendEmailNotificationToNewEmail(user.Email, user.FirstName,
                date, time, appUrl, cancellationToken);

                await service.SendEmailVerified(user.Email, user.FirstName,
                appUrl, cancellationToken);

                await service.SendEmailNotificationToOldEmail(user.Email, user.FirstName, newEmail, date, time);

                
            });
            
        }
    }
}