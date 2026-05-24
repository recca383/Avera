using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Application.Cases;

namespace Avera.Application.Cases.Create
{
    public sealed record CreateCaseCommand
    (
            string SubjectName,
            Guid ExaminerId,
            Priority Priority,
            AnalysisType AnalysisType
    ) : ICommand;
    
}