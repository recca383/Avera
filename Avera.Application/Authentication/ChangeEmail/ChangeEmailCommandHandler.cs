using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.Common;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Authentication.ChangeEmail
{
    internal sealed class ChangeEmailCommandHandler(
         IAuthenticationService authenticationService,
         IUserContext userContext
     ) : ICommandHandler<ChangeEmailCommand, TokenExpiryResponse>
    {
        public async Task<Result<TokenExpiryResponse>> Handle(
            ChangeEmailCommand command,
            CancellationToken cancellationToken)
        {
            return await authenticationService.ChangeEmailAsync(
                userContext.UserId,
                command.NewEmail,
                command.CurrentPassword,
                cancellationToken);
        }
    }
}
