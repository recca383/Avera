using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Tenants.Create
{
    public sealed record CreateTenantResponse(
        string AccessToken,
        DateTime ExpireAt);
}
