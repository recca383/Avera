using Avera.Application.CaseImages;
using Avera.Domain.Application.Cases;
using Avera.Domain.Cases;

namespace Avera.Application.Cases
{
    public sealed record CaseDto
    (
        Guid Id,
        string CaseCode,
        string SubjectName,
        string Examiner,
        Priority Priority,
        DateTime CreatedAt,
        Status CaseStatus,
        DocumentType DocumentType,
        string OptionalDocumentType,
        bool IsDeleted,
        MLResponseDto? MLResponse,
        Guid? ReviewedByUserId,
        DateTimeOffset? ReviewedAt,
        string? ReviewNote,
        FinalVerdict? FinalVerdict,
        bool IsPdfExportAllowed
        ,
        bool ResultViewed
    );

}