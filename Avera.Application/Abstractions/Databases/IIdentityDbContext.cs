using Avera.Domain.Identity.MemberRequests;
using Avera.Domain.Identity.SubscriptionPlans;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.TenantSubscriptions;
using Microsoft.EntityFrameworkCore;

namespace Avera.Application.Abstractions.Databases
{
    public interface IIdentityDbContext
    {
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantSubscription> TenantSubscriptions { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<MemberRequest> MemberRequests { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}