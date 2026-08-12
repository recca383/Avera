using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using SharedKernel;

namespace Avera.Application.Authentication.JoinInviteCode
{
    public sealed class JoinInviteCodeCommandHandler
    (
        IAuthenticationService authenticationService
    ) : ICommandHandler<JoinInviteCodeCommand>
    {
        public async  Task<Result> Handle(JoinInviteCodeCommand command, CancellationToken cancellationToken)
        {
            return await authenticationService.JoinInviteCodeAsync(command.InviteCode, cancellationToken);
        }
    }
}