using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Authentication.ChangeEmail
{
    public sealed record ChangeEmailCommand(
        string NewEmail,
        string CurrentPassword
    ) : ICommand<TokenExpiryResponse>;
}
