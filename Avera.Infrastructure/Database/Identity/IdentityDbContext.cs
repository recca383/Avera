using Avera.Domain.Application.CaseImages;
using Avera.Domain.Application.Cases;
using Avera.Domain.Application.ExportedReports;
using Avera.Domain.Application.Notifications;
using Avera.Infrastructure.Identity;
using Avera.Infrastructure.Identity.InviteCodes;
using Avera.Infrastructure.Identity.Roles;
using Avera.Infrastructure.Identity.SubscriptionPlans;
using Avera.Infrastructure.Identity.Tenants;
using Avera.Infrastructure.Identity.TenantSubscriptions;
using Avera.Infrastructure.Identity.Users;
using Infrastructure.DomainEvents;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Avera.Infrastructure.Database.Identity
{
    public sealed class IdentityDbContext(
        DbContextOptions<IdentityDbContext> options,
        IDomainEventsDispatcher domainEventsDispatcher)
        : IdentityDbContext<User, Role, Guid>(options)
    {
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantSubscription> TenantSubscriptions { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<InviteCode> ShareLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly,
             type => type.Namespace == "Avera.Infrastructure.Database.Identity.Configurations");
            
            //modelBuilder.HasDefaultSchema(Schemas.Default);

            

            modelBuilder.Ignore<Case>();
            modelBuilder.Ignore<CaseImage>();
            modelBuilder.Ignore<ExportedReport>();
            modelBuilder.Ignore<Notification>();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            int result = await base.SaveChangesAsync(cancellationToken);

            await PublishDomainEventsAsync();

            return result;
        }

        private async Task PublishDomainEventsAsync()
        {
            var domainEvents = ChangeTracker
                .Entries<Entity>()
                .Select(entry => entry.Entity)
                .SelectMany(entity =>
                {
                    List<IDomainEvent> domainEvents = entity.DomainEvents;

                    entity.ClearDomainEvents();

                    return domainEvents;
                })
                .ToList();

            await domainEventsDispatcher.DispatchAsync(domainEvents);
        }
    }
}
