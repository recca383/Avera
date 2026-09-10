using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Authentication.ChangeEmail
{
    internal sealed class ChangeEmailCommandHandler(
        IAuthenticationService authenticationService,
        IUserContext userContext
        ) : ICommandHandler<ChangeEmailCommand>
    {
        public async Task<Result> Handle(ChangeEmailCommand command, CancellationToken cancellationToken)
        {
            return await authenticationService.ChangeEmailAsync(
               userContext.UserId,
               command.NewEmail,
               command.CurrentPassword,
               cancellationToken);
        }
    }
}
