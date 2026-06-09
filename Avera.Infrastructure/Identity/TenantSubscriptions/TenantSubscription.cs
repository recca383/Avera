using Avera.Infrastructure.Identity.SubscriptionPlans;
using Avera.Infrastructure.Identity.Tenants;

namespace Avera.Infrastructure.Identity.TenantSubscriptions
{
    public class TenantSubscription
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsTrial { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties

        public Tenant? Tenant {get; set; }
        public Guid TenantId { get; set; }
        public SubscriptionPlan? SubscriptionPlan { get; set; }
        public Guid SubscriptionPlanId { get; set; }
    }
}