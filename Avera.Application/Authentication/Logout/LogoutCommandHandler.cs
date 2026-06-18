using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using SharedKernel;

namespace Avera.Application.Authentication.Logout
{
    public sealed class LogoutCommandHandler 
    (
        IAuthenticationService authenticationServices,
        IUserContext userContext
    ) : ICommandHandler<LogoutCommand>
    {
        public async Task<Result> Handle(LogoutCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await authenticationServices.LogoutAsync(userContext.UserId);

                return Result.Success();
            }
            catch (System.Exception)
            {
                
                return Result.Failure<LogoutCommandHandler>(LogoutCommandError.LogoutError);
            }
        }
    }
}