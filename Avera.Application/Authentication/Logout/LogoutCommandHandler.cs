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
                return await authenticationServices.LogoutAsync(userContext.UserId, cancellationToken);
        }
    }
}