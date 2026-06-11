using Avera.Infrastructure.Identity.SubscriptionPlans;
using Avera.Infrastructure.Identity.Tenants;
using Avera.Infrastructure.Identity.TenantSubscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avera.Infrastructure.Database.Identity.Configurations
{
    internal class TenantSubscriptionConfiguration : IEntityTypeConfiguration<TenantSubscription>
    {
        public void Configure(EntityTypeBuilder<TenantSubscription> builder)
        {
            builder.HasKey(ts => ts.Id);

            builder.HasOne<Tenant>(ts => ts.Tenant)
                .WithMany(t => t.TenantSubscriptions)
                .HasForeignKey(ts => ts.TenantId);

            builder.HasOne<SubscriptionPlan>(ts => ts.SubscriptionPlan)
                .WithMany(sp => sp.TenantSubscriptions)
                .HasForeignKey(ts => ts.SubscriptionPlanId);
        }
    }
}