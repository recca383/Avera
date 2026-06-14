using Avera.Application.CaseImages;
using Avera.Domain.Application.Cases;

namespace Avera.Application.Cases
{
    public sealed record CaseDto
    (
        Guid Id,
        string CaseCode,
        string SubjectName,
        //string Examiner,
        Priority Priority,
        DateTime CreatedAt,
        Status CaseStatus,
        AnalysisType AnalysisType,
        bool IsDeleted
    );

}