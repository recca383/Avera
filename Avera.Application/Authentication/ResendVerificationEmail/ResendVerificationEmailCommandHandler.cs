using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Authentication.ResendVerificationEmail
{
    internal sealed class ResendVerificationEmailCommandHandler(
         IAuthenticationService authenticationService)
    : ICommandHandler<ResendVerificationEmailCommand>
    {
        public Task<Result> Handle(ResendVerificationEmailCommand command, CancellationToken cancellationToken)
        {
            if(command.Type == "change-email")
            {
                return authenticationService.ResendEmailChangeVerificationAsync(command.Email, command.NewEmail!, cancellationToken);
            }

            return authenticationService.ResendVerificationEmailAsync(command.Email, cancellationToken);
        }
    }
}
