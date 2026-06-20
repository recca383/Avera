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
            try
            {
                var userId = userContext.UserId;

                await authenticationService.ResetPasswordAsync(userId, command.Token, command.Password, cancellationToken);
                return Result.Success();
            }
            catch (Exception)
            {
                return Result.Failure(ResetPasswordCommandError.InvalidToken());
            }
        }
    }
}