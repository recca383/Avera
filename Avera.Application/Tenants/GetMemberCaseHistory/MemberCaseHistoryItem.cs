using Avera.Domain.Application.Cases;

namespace Avera.Application.Tenants.GetMemberCaseHistory;

public sealed record MemberCaseHistoryItem(
    Guid Id,
    string CaseCode,
    string SubjectName,
    DateTime CreatedAt,
    Status Status,
    DocumentType DocumentType,
    string? OptionalDocumentType,
    bool IsDeleted);
