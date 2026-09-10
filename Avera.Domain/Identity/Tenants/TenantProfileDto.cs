using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Domain.Identity.Tenants
{
    public sealed record TenantProfileDto(
        Guid Id,
        string Name,
        string InviteCode,
        int MemberCount,
        DateTime CreatedAt
        );
}
