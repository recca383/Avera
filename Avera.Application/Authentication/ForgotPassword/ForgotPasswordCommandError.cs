using SharedKernel;

namespace Avera.Application.Authentication.ForgotPassword
{
    public sealed record ForgotPasswordCommandError : Error
    {
         private ForgotPasswordCommandError(string code, string description, ErrorType errorType)
         : base(code, description, errorType) { }

         public static ForgotPasswordCommandError ForgotPasswordError => new ForgotPasswordCommandError(
            "User.ForgotPassword",
            "Something wrong with the forgot Password, please try again!",
            ErrorType.Failure
         );

        public static ForgotPasswordCommandError EmailNotFound() => new ForgotPasswordCommandError(
            "User.EmailNotFound",
            "Email not found",
            ErrorType.NotFound);
    }
}