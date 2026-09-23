using Avera.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Users.ChangeName
{
    public sealed record ChangeNameCommand(string? NewName, string? NewLastName) : ICommand;
}
