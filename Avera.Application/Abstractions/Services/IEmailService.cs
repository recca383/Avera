using SharedKernel;

namespace Avera.Application.Abstractions.Services
{
    public interface IEmailService
    {
        Task<Result> SendForgotPasswordEmailAsync(
        string recipient,
        string code, 
        string ExpiryInMinutes,
        CancellationToken cancellationToken = default
        );
        Task<Result> SendRequestApprovedAsync(
        string recipient,
        CancellationToken cancellationToken = default
        );
        Task<Result> ResetPasswordNotificationAsync(
        string recipient,
        CancellationToken cancellationToken = default
        );
        Task<Result> SendRequestRejectedAsync(
            string recipient,
            CancellationToken cancellationToken = default);
        Task<Result> SendEmailVerificationAsync(
            string recipient,
            string firstName,
            string verificationUrl,
            CancellationToken cancellationToken = default);

        Task<Result> SendEmailChangeVerificationAsync(
            string recipient,
            string firstName,
            string verificationUrl,
            CancellationToken cancellationToken = default);
        Task<Result> SendEmailVerified(
            string recipient,
            string firstName,
            string appUrl,
            CancellationToken cancellationToken = default);
        Task<Result> SendEmailNotificationToNewEmail(
            string recipient,
            string firstName,
            DateOnly ChangeDate,
            TimeOnly ChangeTime,
            string appUrl,
            CancellationToken cancellationToken = default);
        Task<Result> SendEmailNotificationToOldEmail(
            string recipient,
            string firstName,
            string newEmail,
            DateOnly ChangeDate,
            TimeOnly ChangeTime,
            CancellationToken cancellationToken = default);

    }
}