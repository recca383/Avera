using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Services;
using Avera.Domain.Identity.Users;
using Avera.Infrastructure.Authentication;
using Avera.Infrastructure.Database.Identity;
using Azure.Core;
using FluentEmail.Core;
using FluentEmail.Core.Models;
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
        IDateTimeProvider dateTime
    ) : IEmailService
    {
        private const string HEADER_REFERENCE = "email-header";
        private readonly string CURRENT_YEAR = dateTime.PhilippineNow.Year.ToString();

        public async Task<Result> ResetPasswordNotificationAsync(
            string recipient,
            string firstName,
            string organizationName,
            string adminFullName,
            string supportEmail,
            CancellationToken cancellationToken = default)
        {

            var result =  await fluentEmail
                .To(recipient)
                .Subject("Reset Password Notification")
                .UsingTemplateFromFile(
                    GetTemplatePath("reset-password-notification.cshtml"),
                    new
                    {
                        Header = HEADER_REFERENCE,
                        FirstName = firstName,
                        Email = recipient,
                        ChangeDate = dateTime.PhilippineNow.ToString("MMMM dd, yyyy"),
                        ChangeTime = dateTime.PhilippineNow.ToLongTimeString(),
                        OrganizationName = organizationName,
                        AdminName = adminFullName,
                        SupportEmail = supportEmail,
                        CurrentYear = CURRENT_YEAR
                    }
                )
                .Attach(GetHeader())
                .SendAsync(cancellationToken);

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

        public async Task<Result> SendForgotPasswordEmailAsync(
            string recipient,
            string firstName,
            string code,
            string supportEmail,
            int ExpiryInMinutes,
            CancellationToken cancellationToken = default)
        { 
            var result =  await fluentEmail
                .To(recipient)
                .Subject("Forgot Password")
                .UsingTemplateFromFile(
                    GetTemplatePath("forgot-password.cshtml"),
                    new
                    {
                        Header = HEADER_REFERENCE,
                        FirstName = firstName,
                        Email = recipient,
                        Code = code,
                        ExpiryMinutes = ExpiryInMinutes,
                        SupportEmail = supportEmail,
                        CurrentYear = CURRENT_YEAR
                    }
                )
                .Attach(GetHeader())
                .SendAsync(cancellationToken);

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

        public async Task<Result> SendRequestApprovedAsync(
            string recipient,
            string firstName,
            string organizationName, 
            string adminFullName, 
            CancellationToken cancellationToken = default)
        {
            var result =  await fluentEmail
                .To(recipient)
                .Subject("Request Approved")
                .UsingTemplateFromFile(
                    GetTemplatePath("request-approved.cshtml"),
                    new
                    {
                        Header = HEADER_REFERENCE,
                        FirstName = firstName,
                        OrganizationName = organizationName,
                        AdminName = adminFullName,
                        CurrentYear = CURRENT_YEAR
                    }
                )
                .Attach(GetHeader())
                .SendAsync(cancellationToken);

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

        public async Task<Result> SendRequestRejectedAsync(
            string recipient,
            string firstName,
            string organizationName,
            string adminFullName,
            string supportEmail,
            CancellationToken cancellationToken = default)
        {
            var result =  await fluentEmail
                .To(recipient)
                .Subject("Request Rejected")
                .UsingTemplateFromFile(
                    GetTemplatePath("request-rejected.cshtml"),
                    new
                    {
                        Header = HEADER_REFERENCE,
                        FirstName = firstName,
                        OrganizationName = organizationName,
                        AdminName = adminFullName,
                        CurrentYear = CURRENT_YEAR,
                        SupportEmail = supportEmail
                    }
                )
                .Attach(GetHeader())
                .SendAsync(cancellationToken);
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

        public async Task<Result> SendEmailVerificationAsync(
            string recipient, 
            string firstName,
            string verificationUrl,
            string supportEmail,
            int expiryHours,
            CancellationToken cancellationToken = default)
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
                       SupportEmail = supportEmail,
                       CurrentYear =CURRENT_YEAR,
                       ExpiryHours = expiryHours
                   })
               .Attach(GetHeader())
               .SendAsync(cancellationToken);

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

        public async Task<Result> SendEmailChangeVerificationAsync(
            string recipient,
            string supportEmail,
            string firstName, 
            string verificationUrl, 
            int expiryHours,
            CancellationToken cancellationToken = default)
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
                       SupportEmail = supportEmail,
                       CurrentYear = CURRENT_YEAR,
                       ExpiryHours = expiryHours
                   })
               .Attach(GetHeader())
               .SendAsync(cancellationToken);

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

        public async Task<Result> SendEmailNotificationToNewEmail(
            string recipient,
            string firstName,
            DateOnly ChangeDate,
            TimeOnly ChangeTime,
            string appUrl,
            CancellationToken cancellationToken = default)
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
                       CurrentYear = CURRENT_YEAR,
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
        
        public async Task<Result> SendEmailVerified(
            string recipient, 
            string firstName,
            string appUrl,
            CancellationToken cancellationToken = default)
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
                       AppUrl = appUrl,
                       EmailAddress = recipient,
                       CurrentYear = CURRENT_YEAR,
                   })
               .Attach(GetHeader())
               .SendAsync(cancellationToken);

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

        public async Task<Result> SendEmailNotificationToOldEmail(
            string recipient,
            string firstName,
            string newEmail, 
            DateOnly changeDate, 
            TimeOnly changeTime, 
            string supportEmail,
            CancellationToken cancellationToken = default)
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
                       ChangeDate = changeDate,
                       ChangeTime = changeTime,
                       SupportEmail = supportEmail,
                       CurrentYear = CURRENT_YEAR,
                   })
               .Attach(GetHeader())
               .SendAsync(cancellationToken);

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
        private static Attachment GetHeader()
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