using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Application.Cases;

namespace Avera.Application.Cases.Get
{
    public sealed record GetCasesQuery(
        Status? CaseStatus,
        Priority? AnalysisPriority,
        DocumentType? AnalysisType,
        int? Page,
        int? PageSize
    ) : IQuery<GetCasesQueryResult>;
}