using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using SharedKernel;

namespace Avera.Application.Authentication.ChangePassword
{
    public sealed class ChangePasswordCommandHandler
    (
        IAuthenticationService authenticationService,
        IUserContext userContext
    ) : ICommandHandler<ChangePasswordCommand>
    {
        public async Task<Result> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
        {

            return await authenticationService.ChangePasswordAsync(
                userContext.UserId,
                command.currentPassword,
                command.newPassword,
                cancellationToken);
        }
    }
}