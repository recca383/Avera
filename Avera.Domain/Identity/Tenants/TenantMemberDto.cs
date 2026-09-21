namespace Avera.Domain.Identity.Tenants
{
    public sealed record TenantMemberDto(
    Guid Id,
    string? FirstName,
    string? LastName,
    string Email,
    string Role,
    bool IsSuspended,
    int CasesHandled,
    int DailyCaseLimit,
    DateTime? JoinedAt
);
}