using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using SharedKernel;
using Avera.Domain.Identity.Users;

namespace Avera.Application.Authentication.VerifyEmail
{
    internal sealed class VerifyEmailCommandHandler(
        IAuthenticationService authenticationService
    ) : ICommandHandler<VerifyEmailCommand, string>
    {
        public async Task<Result<string>> Handle(VerifyEmailCommand command, CancellationToken cancellationToken)
        {
            if (command.Type == "change-email")
            {
                if (string.IsNullOrWhiteSpace(command.Email))
                    return Result.Failure<string>(UserErrors.InvalidEmail);

                return await authenticationService.VerifyEmailChangeAsync(
                    command.UserId,
                    command.Email,
                    command.Token,
                    cancellationToken
                    );
            }

            return await authenticationService.VerifyEmailAsync(
                command.UserId,
                command.Token,
                cancellationToken);
        }
    }
}