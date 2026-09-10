using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Identity.Tenants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Tenants.GetInviteCode
{
    public sealed record GetTenantInviteCodeQuery : IQuery<TenantInviteCodeDto>;
}
