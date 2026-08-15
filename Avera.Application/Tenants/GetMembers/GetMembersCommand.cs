using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Identity.Tenants;

namespace Avera.Application.Tenants.GetMembers
{
    public sealed record GetMembersCommand(
        bool? IsAlphabetical,
        bool? IsMostCases,
        string? Name
    ) : ICommand<List<TenantMemberDto>>;
}