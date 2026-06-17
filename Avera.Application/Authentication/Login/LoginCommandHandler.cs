using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using SharedKernel;

namespace Avera.Application.Authentication.Login
{
    public sealed class LoginCommandHandler(IAuthenticationService authenticationServices) 
    : ICommandHandler<LoginCommand, LoginResponse>
    {
        public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            return await authenticationServices.LoginAsync(
                command.Email,
                command.Password,
                cancellationToken);
        }
    }
}