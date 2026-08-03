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
    }
}