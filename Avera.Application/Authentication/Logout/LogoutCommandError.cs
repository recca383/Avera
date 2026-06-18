using SharedKernel;

namespace Avera.Application.Authentication.Logout
{
    public sealed record LogoutCommandError : Error
    {
        private LogoutCommandError(string code, string description, ErrorType type) : base(code, description, type)
        {
        }

        public static LogoutCommandError LogoutError => new LogoutCommandError(
            "User.Logout",
            "Something went wrong on logout",
            ErrorType.Failure
        );

    }
}