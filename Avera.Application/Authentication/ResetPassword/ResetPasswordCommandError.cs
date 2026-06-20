using SharedKernel;

namespace Avera.Application.Authentication.ResetPassword
{
    public sealed record ResetPasswordCommandError : Error
    {
        private ResetPasswordCommandError(string code, string description, ErrorType type) : base(code, description, type)
        {
        }

        public static ResetPasswordCommandError InvalidToken() => new ResetPasswordCommandError(
            code: "InvalidToken",
            description: "The provided token is invalid or has expired.",
            type: ErrorType.Validation
        );

        public static ResetPasswordCommandError UserNotFound() => new ResetPasswordCommandError(
            code: "UserNotFound",
            description: "The user associated with the provided token was not found.",
            type: ErrorType.NotFound
        );
    }
}