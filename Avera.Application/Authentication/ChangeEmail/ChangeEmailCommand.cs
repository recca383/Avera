using Avera.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Authentication.ChangeEmail
{
    public sealed record ChangeEmailCommand(
        string NewEmail,
        string CurrentPassword
        ) : ICommand;
}
