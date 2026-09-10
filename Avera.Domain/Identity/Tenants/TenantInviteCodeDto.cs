using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Domain.Identity.Tenants
{
    public sealed record TenantInviteCodeDto(
        string InviteCode
        );
}
