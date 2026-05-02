using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avera.Application.Abstractions.Databases;
using Avera.Domain.Identity.RoleClaims;
using Avera.Domain.Identity.Roles;
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
    public sealed class ApplicationIdentityDbContext(
        DbContextOptions<ApplicationIdentityDbContext> options,
        IDomainEventsDispatcher domainEventsDispatcher)
        : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>(options), IApplicationIdentityDbContext
    {
        DbSet<Tenant> IApplicationIdentityDbContext.Tenants { get; set; }
        DbSet<TenantSubscription> IApplicationIdentityDbContext.TenantSubscriptions { get; set; }
        DbSet<SubscriptionPlan> IApplicationIdentityDbContext.SubscriptionPlans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationIdentityDbContext).Assembly);
            
            modelBuilder.HasDefaultSchema(Schemas.Default);
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
