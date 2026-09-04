using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Domain.Application.CaseImages;
using Avera.Domain.Application.Cases;
using Avera.Domain.Application.ExportedReports;
using Avera.Domain.Application.Notifications;
using Avera.Domain.Identity.MemberRequests;
using Avera.Domain.Identity.Roles;
using Avera.Domain.Identity.SubscriptionPlans;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.TenantSubscriptions;
using Avera.Domain.Identity.Users;
using Avera.Infrastructure.Identity;
using Infrastructure.DomainEvents;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Avera.Infrastructure.Database.Identity
{
    public sealed class IdentityDbContext(
        DbContextOptions<IdentityDbContext> options,
        IDomainEventsDispatcher domainEventsDispatcher,
        IUserContext userContext)
        : IdentityDbContext<User, Role, Guid>(options), IIdentityDbContext
    {
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantSubscription> TenantSubscriptions { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<MemberRequest> MemberRequests { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly,
             type => type.Namespace == "Avera.Infrastructure.Database.Identity.Configurations");
            
            //modelBuilder.HasDefaultSchema(Schemas.Default);

            modelBuilder.Entity<User>().HasQueryFilter(u => u.TenantId == userContext.TenantId);
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
