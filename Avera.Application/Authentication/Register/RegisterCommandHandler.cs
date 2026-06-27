using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using SharedKernel;

namespace Avera.Application.Authentication.Register
{
    public sealed class RegisterCommandHandler(IAuthenticationService authenticationServices) : ICommandHandler<RegisterCommand>
    {
        public async Task<Result> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            var request = new RegisterRequest
                (
                    command.FirstName,
                    command.LastName,
                    command.Email,
                    command.Password,
                    command.Role
                );
            return await authenticationServices.RegisterAsync(
                request,
                cancellationToken
            );
        }
    }
}