using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Authentication.VerifyPasswordCode
{
    internal sealed class VerifyPasswordCodeCommandHandler(IAuthenticationService authenticationService) : ICommandHandler<VerifyPasswordCodeCommand>
    {
        public Task<Result> Handle(VerifyPasswordCodeCommand command, CancellationToken cancellationToken)
        {
            return authenticationService.VerifyPasswordResetCodeAsync(command.Email, command.Code, cancellationToken);
        }
    }
}
