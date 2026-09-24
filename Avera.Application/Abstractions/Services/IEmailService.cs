using SharedKernel;

namespace Avera.Application.Abstractions.Services
{
    public interface IEmailService
    {
        Task<Result> ResetPasswordNotificationAsync(
            string recipient,
            string firstName,
            string organizationName,
            string adminFullName,
            string supportEmail,
            CancellationToken cancellationToken = default
        );
        Task<Result> SendForgotPasswordEmailAsync(
            string recipient,
            string firstName,
            string code,
            string supportEmail,
            int ExpiryInMinutes,
            CancellationToken cancellationToken = default
        );
        Task<Result> SendRequestApprovedAsync(
            string recipient,
            string firstName,
            string organizationName,
            string adminFullName,
            CancellationToken cancellationToken = default
        );
        
        Task<Result> SendRequestRejectedAsync(
            string recipient,
            string firstName,
            string organizationName,
            string adminFullName,
            string supportEmail,
            CancellationToken cancellationToken = default);
        Task<Result> SendEmailVerificationAsync(
            string recipient,
            string firstName,
            string verificationUrl,
            string supportEmail,
            int expiryHours,
            CancellationToken cancellationToken = default);

        Task<Result> SendEmailChangeVerificationAsync(
            string recipient,
            string supportEmail,
            string firstName,
            string verificationUrl,
            int expiryHours,
            CancellationToken cancellationToken = default);

        Task<Result> SendEmailNotificationToNewEmail(
            string recipient,
            string firstName,
            DateOnly ChangeDate,
            TimeOnly ChangeTime,
            string appUrl,
            CancellationToken cancellationToken = default);

        Task<Result> SendEmailVerified(
            string recipient,
            string firstName,
            string appUrl,
            CancellationToken cancellationToken = default);
        
        Task<Result> SendEmailNotificationToOldEmail(
            string recipient,
            string firstName,
            string newEmail,
            DateOnly changeDate,
            TimeOnly changeTime,
            string supportEmail,
            CancellationToken cancellationToken = default);

        Task<Result<string>> SendVerifiedFallback(
            CancellationToken cancellationToken = default);

    }
}