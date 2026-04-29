using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avera.Domain.Identity.SubscriptionPlans;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.TenantSubscriptions;
using Microsoft.EntityFrameworkCore;

namespace Avera.Application.Abstractions.Databases
{
    public interface IApplicationIdentityDbContext
    {

        DbSet<Tenant> Tenants { get; set; }
        DbSet<TenantSubscription> TenantSubscriptions {get; set; }
        DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
