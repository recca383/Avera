using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using SharedKernel;

namespace Avera.Application.Authentication.VerifyEmail
{
    internal sealed class VerifyEmailCommandHandler(
        IAuthenticationService authenticationService
    ) : ICommandHandler<VerifyEmailCommand>
    {
        public async Task<Result> Handle(
            VerifyEmailCommand command,
            CancellationToken cancellationToken)
        {
            return await authenticationService.VerifyEmailAsync(
                command.UserId,
                command.Token,
                cancellationToken);
        }
    }
}