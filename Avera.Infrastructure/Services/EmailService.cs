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

        public async Task<Result> SendEmailVerificationAsync(string recipient, string firstName, string verificationUrl, CancellationToken cancellationToken = default)
        {
            var result = await fluentEmail
               .To(recipient)
               .Subject("Verify Your Avera Email")
               .UsingTemplateFromFile(
                   GetTemplatePath("verify-email.cshtml"),
                   new
                   {
                       Header = HEADER_REFERENCE,
                       FirstName = firstName,
                       VerificationUrl = verificationUrl,
                       SupportEmail = SUPPORT_EMAIL,
                       CurrentYear = DateTime.UtcNow.Year,
                       ExpiryHours = 1
                   })
               .Attach(GetHeader())
               .SendAsync();

            if (!result.Successful)
            {
                var errors = result.ErrorMessages
                    .Select(e => new Error(
                        "Email.Failure",
                        e,
                        ErrorType.Failure))
                    .ToArray();

                return Result.Failure(
                    new ValidationError(errors));
            }

            return Result.Success();
        }
        public async Task<Result> SendEmailChangeVerificationAsync(string recipient, string firstName, string verificationUrl, CancellationToken cancellationToken = default)
        {
            var result = await fluentEmail
               .To(recipient)
               .Subject("Confirm Your New Avera Email")
               .UsingTemplateFromFile(
                   GetTemplatePath("change-email-confirmation.cshtml"),
                   new
                   {
                       Header = HEADER_REFERENCE,
                       FirstName = firstName,
                       NewEmail = recipient,
                       ConfirmationUrl = verificationUrl,
                       SupportEmail = SUPPORT_EMAIL,
                       CurrentYear = DateTime.UtcNow.Year,
                       ExpiryHours = 1
                   })
               .Attach(GetHeader())
               .SendAsync();

            if (!result.Successful)
            {
                var errors = result.ErrorMessages
                    .Select(e => new Error(
                        "Email.Failure",
                        e,
                        ErrorType.Failure))
                    .ToArray();

                return Result.Failure(
                    new ValidationError(errors));
            }

            return Result.Success();
        }

        public async Task<Result> SendEmailNotificationToNewEmail(string recipient, string firstName, DateOnly ChangeDate, TimeOnly ChangeTime, string appUrl, CancellationToken cancellationToken = default)
        {
            var result = await fluentEmail
               .To(recipient)
               .Subject("Your New Avera Email Is Confirmed")
               .UsingTemplateFromFile(
                   GetTemplatePath("email-changed-notification-new.cshtml"),
                   new
                   {
                       Header = HEADER_REFERENCE,
                       FirstName = firstName,
                       NewEmail = recipient,
                       ChangeDate = ChangeDate.ToString("MMMM dd, yyyy"),
                       ChangeTime = ChangeTime.ToString("hh:mm tt"),
                       CurrentYear = DateTime.UtcNow.Year,
                       AppUrl = appUrl,
                   })
               .Attach(GetHeader())
               .SendAsync();

            if (!result.Successful)
            {
                var errors = result.ErrorMessages
                    .Select(e => new Error(
                        "Email.Failure",
                        e,
                        ErrorType.Failure))
                    .ToArray();

                return Result.Failure(
                    new ValidationError(errors));
            }

            return Result.Success();
        }
        
        public async Task<Result> SendEmailVerified(string recipient, string firstName, string AppUrl, CancellationToken cancellationToken = default)
        {
            var result = await fluentEmail
               .To(recipient)
               .Subject("Your Avera Email Has Been Verified")
               .UsingTemplateFromFile(
                   GetTemplatePath("email-verified.cshtml"),
                   new
                   {
                       Header = HEADER_REFERENCE,
                       FirstName = firstName,
                       AppUrl = AppUrl,
                       EmailAddress = recipient,
                       CurrentYear = DateTime.UtcNow.Year,
                   })
               .Attach(GetHeader())
               .SendAsync();

            if (!result.Successful)
            {
                var errors = result.ErrorMessages
                    .Select(e => new Error(
                        "Email.Failure",
                        e,
                        ErrorType.Failure))
                    .ToArray();

                return Result.Failure(
                    new ValidationError(errors));
            }

            return Result.Success();
        }

        public async Task<Result> SendEmailNotificationToOldEmail(string recipient, string firstName, string newEmail, DateOnly ChangeDate, TimeOnly ChangeTime, CancellationToken cancellationToken = default)
        {
            var result = await fluentEmail
               .To(recipient)
               .Subject("Your Avera Account Email Was Changed")
               .UsingTemplateFromFile(
                   GetTemplatePath("email-changed-notification-old.cshtml"),
                   new
                   {
                       Header = HEADER_REFERENCE,
                       FirstName = firstName,
                       OldEmail = recipient,
                       NewEmail = newEmail,
                       ChangeDate = ChangeDate,
                       ChangeTime = ChangeTime,
                       SupportEmail = SUPPORT_EMAIL,
                       CurrentYear = DateTime.UtcNow.Year,
                   })
               .Attach(GetHeader())
               .SendAsync();

            if (!result.Successful)
            {
                var errors = result.ErrorMessages
                    .Select(e => new Error(
                        "Email.Failure",
                        e,
                        ErrorType.Failure))
                    .ToArray();

                return Result.Failure(
                    new ValidationError(errors));
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