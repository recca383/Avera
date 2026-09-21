using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.Common;
using Avera.Domain.Identity.Users;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Authentication.ResendVerificationEmail
{
    internal sealed class ResendVerificationEmailCommandHandler(
         IAuthenticationService authenticationService)
    : ICommandHandler<ResendVerificationEmailCommand, TokenExpiryResponse>
    {
        public async Task<Result<TokenExpiryResponse>> Handle(
        ResendVerificationEmailCommand command,
        CancellationToken cancellationToken)
        {
            if (command.Type == "change-email")
            {
                if (string.IsNullOrWhiteSpace(command.NewEmail))
                {
                    return Result.Failure<TokenExpiryResponse>(
                        UserErrors.InvalidEmail);
                }

                return await authenticationService
                    .ResendEmailChangeVerificationAsync(
                        command.Email,
                        command.NewEmail,
                        cancellationToken);
            }

            return await authenticationService
                .ResendVerificationEmailAsync(
                    command.Email,
                    cancellationToken);
        }

    }
}
