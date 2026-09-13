using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Cases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Cases.Review
{
    public sealed record ReviewCaseCommand(
        Guid CaseId,
        FinalVerdict FinalVerdict,
        string? ReviewNote,
        bool IsPdfExportAllowed
    ) : ICommand;
}
