using SharedKernel;

namespace Avera.Application.Authentication.Login
{
    public sealed record LoginCommandError : Error
    {
        private LoginCommandError(string code, string description, ErrorType type) : base(code, description, type)
        {
        }

        public static LoginCommandError EmailNotFound() => new LoginCommandError(
            "Login.EmailNotFound",
            "The email address provided does not exist.",
            ErrorType.NotFound
        );

        public static LoginCommandError InvalidCredentials() => new LoginCommandError(
            "Login.InvalidCredentials",
            "The email or password provided is incorrect.",
            ErrorType.Validation
        );
    }
}