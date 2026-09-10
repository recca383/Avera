using Avera.Application.Authentication.Login;
using Avera.Application.Authentication.Register;
using SharedKernel;

namespace Avera.Application.Abstractions.Authentication
{
    public interface IAuthenticationService
    {
         Task<Result<LoginResponse>> LoginAsync(
            string email,
            string password,
            CancellationToken cancellationToken = default);
        Task<Result> RegisterAsync(
            RegisterRequest request,
            CancellationToken cancellationToken = default);
        Task<Result> LogoutAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
        Task<Result> ForgotPasswordAsync(
            string email,
            CancellationToken cancellationToken = default);
        Task<Result> ResetPasswordAsync(
            string email,
            string token,
            string password,
            CancellationToken cancellationToken = default);
        Task<Result> JoinInviteCodeAsync(
        string inviteCode,
        CancellationToken cancellationToken = default);
        Task<Result> ChangePasswordAsync(
            Guid userId,
            string currentPassword,
            string newPassword,
            CancellationToken cancellationToken = default);
        Task<Result> DeleteUserAsync(
            Guid userId,
            CancellationToken cancellationToken
        );
        Task<Result> VerifyPasswordResetCodeAsync(
            string email,
            string code,
            CancellationToken cancellationToken = default
        );
        Task<Result> VerifyEmailAsync(
            Guid userId,
            string token,
            CancellationToken cancellationToken
        );
        Task<Result> ChangeEmailAsync(
            Guid userId,
            string newEmail,
            string currentPassword,
            CancellationToken cancellationToken = default
        );

        Task<Result> VerifyEmailChangeAsync(
            Guid userId,
            string newEmail,
            string token,
            CancellationToken cancellationToken = default);
    }
}