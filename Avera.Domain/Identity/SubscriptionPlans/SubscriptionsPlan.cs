using Avera.Domain.Identity.TenantSubscriptions;

namespace Avera.Domain.Identity.SubscriptionPlans
{
    public class SubscriptionPlan
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "No Display Name";
        public string Description { get; set; } = "No Description";
        public decimal PriceMonthly { get; set; }
        public decimal PriceYearly { get; set; }
        public int MaxAnalyst { get; set; }
        public int MaxCasesPerMonth { get; set; }
        public int StorageGb { get; set; }
        public bool HasPrioritySupport { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set;}

        // Navigation Properties
        public List<TenantSubscription> TenantSubscriptions { get; set; } = new();
    }
}