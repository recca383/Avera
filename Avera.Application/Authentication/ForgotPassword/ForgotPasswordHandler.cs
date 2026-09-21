using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.Common;
using Microsoft.Extensions.Configuration;
using SharedKernel;

namespace Avera.Application.Authentication.ForgotPassword
{
    public sealed class ForgotPasswordHandler
    (
        IAuthenticationService authenticationServices,
        IConfiguration configuration

    ) : ICommandHandler<ForgotPasswordCommand, TokenExpiryResponse>
    {
        public async Task<Result<TokenExpiryResponse>> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            //return await authenticationServices.ForgotPasswordAsync(
            //   command.Email,
            //   cancellationToken);
        }
    }
}