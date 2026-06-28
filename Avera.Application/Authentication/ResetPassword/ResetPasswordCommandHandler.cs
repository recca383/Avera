using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using SharedKernel;

namespace Avera.Application.Authentication.ResetPassword
{
    public sealed class ResetPasswordCommandHandler
    (
        IAuthenticationService authenticationService,
        IUserContext userContext
    ) : ICommandHandler<ResetPasswordCommand>
    {
        public async Task<Result> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
                var userId = userContext.UserId;

                return  await authenticationService.ResetPasswordAsync(userId, command.Token, command.Password, cancellationToken);
                
        }
    }
}