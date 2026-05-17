using Avera.Application.Messaging;
using Avera.Domain.Application.Cases;

namespace Avera.Application.Cases.Get
{
    public sealed class GetCasesQuery : IQuery<GetCasesQueryResult>
    {
        public Status? CaseStatus { get; set; }
        public Priority? AnalysisPriority { get; set; }
        public AnalysisType? AnalysisType { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}