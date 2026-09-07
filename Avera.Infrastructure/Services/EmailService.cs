using System.Net.Mail;
using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Services;
using Avera.Domain.Identity.Users;
using Avera.Infrastructure.Authentication;
using Avera.Infrastructure.Database.Identity;
using Azure.Core;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;

namespace Avera.Infrastructure.Services
{
    public class EmailService
    (
        IFluentEmail fluentEmail,
        UserManager<User> userManager,
        IIdentityDbContext identityDbContext,
        IUserContext userContext
    ) : IEmailService
    {
        private const string HEADER_REFERENCE = "email-header";
        private readonly string CURRENT_YEAR = DateTime.UtcNow.Year.ToString();
        private readonly string DEEP_LINK_URL = "avera://";
        private readonly string SUPPORT_EMAIL = "sirpatrick121402@gmail.com";

        public async Task<Result> ResetPasswordNotificationAsync(string recipient, CancellationToken cancellationToken = default)
        {
            var userRecipient = await userManager.FindByEmailAsync(recipient);
            var organization = await identityDbContext.Tenants
                    .FirstOrDefaultAsync(t => t.Id == userContext.TenantId, cancellationToken);
            var admin = await userManager.FindByIdAsync(userContext.UserId.ToString());

            var result =  await fluentEmail
                .To(recipient)
                .Subject("Reset Password Notification")
                .UsingTemplateFromFile(
                    GetTemplatePath("reset-password-notification.cshtml"),
                    new
                    {
                        Header = HEADER_REFERENCE,
                        FirstName = userRecipient!.FirstName,
                        Email = recipient,
                        ChangeDate = DateTime.UtcNow.ToString("MMMM dd, yyyy"),
                        ChangeTime = DateTime.UtcNow.ToLongTimeString(),
                        OrganizationName = organization!.Name,
                        AdminName = admin!.FirstName + " " + admin!.LastName,
                        SupportEmail = admin.Email,
                        CurrentYear = CURRENT_YEAR
                    }
                )
                .Attach(GetHeader())
                .SendAsync();

            if(!result.Successful)
            {
                var errors = result.ErrorMessages.Select(
                e => new Error(
                    "Email.Failure",
                    e,
                    ErrorType.Failure)
                ).
                ToArray();

                var validationErrors = new ValidationError(errors);
                return Result.Failure(validationErrors);
            }

            return Result.Success();
        }

        public async Task<Result> SendForgotPasswordEmailAsync(string recipient, string code, string ExpiryInMinutes, CancellationToken cancellationToken = default)
        {
            var userRecipient = await userManager.FindByEmailAsync(recipient);
            var organization = await identityDbContext.Tenants
                    .FirstOrDefaultAsync(t => t.Id == userContext.TenantId, cancellationToken);
            var admin = await userManager.FindByIdAsync(userContext.UserId.ToString());

            var result =  await fluentEmail
                .To(recipient)
                .Subject("Forgot Password")
                .UsingTemplateFromFile(
                    GetTemplatePath("forgot-password.cshtml"),
                    new
                    {
                        Header = HEADER_REFERENCE,
                        FirstName = userRecipient!.FirstName,
                        Email = recipient,
                        Code = code,
                        ExpiryMinutes = ExpiryInMinutes,
                        SupportEmail = admin!.Email,
                        CurrentYear = CURRENT_YEAR
                    }
                )
                .Attach(GetHeader())
                .SendAsync();

            if(!result.Successful)
            {
                var errors = result.ErrorMessages.Select(
                e => new Error(
                    "Email.Failure",
                    e,
                    ErrorType.Failure)
                ).
                ToArray();

                var validationErrors = new ValidationError(errors);
                return Result.Failure(validationErrors);
            }

            return Result.Success();
        }

        public async Task<Result> SendRequestApprovedAsync(string recipient, CancellationToken cancellationToken = default)
        {
            var userRecipient = await userManager.FindByEmailAsync(recipient);
            var organization = await identityDbContext.Tenants
                    .FirstOrDefaultAsync(t => t.Id == userContext.TenantId, cancellationToken);
            var admin = await userManager.FindByIdAsync(userContext.UserId.ToString());

            var result =  await fluentEmail
                .To(recipient)
                .Subject("Request Approved")
                .UsingTemplateFromFile(
                    GetTemplatePath("request-approved.cshtml"),
                    new
                    {
                        Header = HEADER_REFERENCE,
                        FirstName = userRecipient!.FirstName,
                        OrganizationName = organization!.Name,
                        AdminName = admin!.FirstName + " " + admin!.LastName,
                        CurrentYear = CURRENT_YEAR
                    }
                )
                .Attach(GetHeader())
                .SendAsync();

            if(!result.Successful)
            {
                var errors = result.ErrorMessages.Select(
                e => new Error(
                    "Email.Failure",
                    e,
                    ErrorType.Failure)
                ).
                ToArray();

                var validationErrors = new ValidationError(errors);
                return Result.Failure(validationErrors);
            }

            return Result.Success();
        }

        public async Task<Result> SendRequestRejectedAsync(string recipient, CancellationToken cancellationToken = default)
        {
            var userRecipient = await userManager.FindByEmailAsync(recipient);
            var organization = await identityDbContext.Tenants
                    .FirstOrDefaultAsync(t => t.Id == userContext.TenantId, cancellationToken);
            var admin = await userManager.FindByIdAsync(userContext.UserId.ToString());
            var result =  await fluentEmail
                .To(recipient)
                .Subject("Request Rejected")
                .UsingTemplateFromFile(
                    GetTemplatePath("request-rejected.cshtml"),
                    new
                    {
                        Header = HEADER_REFERENCE,
                        FirstName = userRecipient!.FirstName,
                        OrganizationName = "Philippine National Police", //organization!.Name,
                        AdminName = "Patrick Fernandez",//admin!.FirstName + " " + admin!.LastName,
                        CurrentYear = CURRENT_YEAR,
                        LoginUrl = DEEP_LINK_URL,
                        SupportEmail = SUPPORT_EMAIL
                    }
                )
                .Attach(GetHeader())
                .SendAsync();
            if(!result.Successful)
            {
                var errors = result.ErrorMessages.Select(
                e => new Error(
                    "Email.Failure",
                    e,
                    ErrorType.Failure)
                ).
                ToArray();
                var validationErrors = new ValidationError(errors);
                return Result.Failure(validationErrors);
            }
            return Result.Success();
        }

         private static string GetTemplatePath(string templateName)
        {
            var assemblyLocation =
                Path.GetDirectoryName(
                    typeof(EmailService).Assembly.Location)!;

            return Path.Combine(
                assemblyLocation,
                "EmailTemplates",
                templateName);
        }
        private FluentEmail.Core.Models.Attachment GetHeader()
        {
            string HEADER_PATH = Path.Combine(Path.GetDirectoryName(typeof(EmailService).Assembly.Location)!,
                                    "EmailTemplates",
                                    "Email Header.png");

            return new FluentEmail.Core.Models.Attachment
            {
                Data = File.OpenRead(HEADER_PATH),
                Filename = "Email Header.png",
                ContentType = "image/png",
                IsInline = true,
                ContentId = HEADER_REFERENCE
            };
        }
    }
}