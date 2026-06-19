using SharedKernel;

namespace Avera.Application.Authentication.Register
{
    public sealed record RegisterCommandError : Error
    {
        private RegisterCommandError(string code, string message, ErrorType errorType) : base(code, message, errorType)
        {

        }

        public static RegisterCommandError RegisterError => new RegisterCommandError(
            "User.Register",
            "Error on Registration",
            ErrorType.Failure
        );

        public static RegisterCommandError UserCreationFailed => new RegisterCommandError(
            "User.CreationFailed",
            "User creation failed",
            ErrorType.Failure
        );

        public static RegisterCommandError RoleAssignmentFailed => new RegisterCommandError(
            "User.RoleAssignmentFailed",
            "Role assignment failed",
            ErrorType.Failure
        );
         
    }
}