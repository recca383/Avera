using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Identity.Tenants;

namespace Avera.Application.Tenants.GetMembers
{
    public sealed record GetMembersQuery(
        bool? IsAlphabetical,
        bool? IsMostCases,
        string? Name
    ) : IQuery<List<TenantMemberDto>>;
}