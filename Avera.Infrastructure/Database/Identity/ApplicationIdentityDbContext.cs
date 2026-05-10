using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avera.Application.Abstractions.Databases;
using Avera.Domain.Application.CaseImages;
using Avera.Domain.Application.Cases;
using Avera.Domain.Application.ExportedReports;
using Avera.Domain.Application.Notifications;
using Avera.Domain.Identity.RoleClaims;
using Avera.Domain.Identity.Roles;
using Avera.Domain.Identity.ShareLinks;
using Avera.Domain.Identity.SubscriptionPlans;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.TenantSubscriptions;
using Avera.Domain.Identity.UserClaims;
using Avera.Domain.Identity.UserLogins;
using Avera.Domain.Identity.UserRoles;
using Avera.Domain.Identity.Users;
using Avera.Domain.Identity.UserTokens;
using Infrastructure.Database;
using Infrastructure.DomainEvents;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Avera.Infrastructure.Database.Identity
{
    internal sealed class ApplicationIdentityDbContext(
        DbContextOptions<ApplicationIdentityDbContext> options,
        IDomainEventsDispatcher domainEventsDispatcher)
        : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>(options), IApplicationIdentityDbContext
    {
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantSubscription> TenantSubscriptions { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<ShareLink> ShareLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationIdentityDbContext).Assembly,
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
