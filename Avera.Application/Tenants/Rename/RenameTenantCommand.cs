using Avera.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Tenants.Rename
{
    public sealed record RenameTenantCommand(
        string newName
        ) : ICommand;
}
