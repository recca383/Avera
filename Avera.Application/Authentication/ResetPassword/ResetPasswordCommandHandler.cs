using System.Net;
using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using SharedKernel;

namespace Avera.Application.Authentication.ResetPassword
{
    public sealed class ResetPasswordCommandHandler
    (
        IAuthenticationService authenticationService
    ) : ICommandHandler<ResetPasswordCommand>
    {
        public async Task<Result> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
                return  await authenticationService.ResetPasswordAsync(command.Email, command.Token, command.Password, cancellationToken);
                
        }
    }
}