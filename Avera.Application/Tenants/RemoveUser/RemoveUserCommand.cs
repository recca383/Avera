using Avera.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Tenants.RemoveFromTenant
{
    public sealed record RemoveUserCommand(Guid userId) : ICommand;
}
