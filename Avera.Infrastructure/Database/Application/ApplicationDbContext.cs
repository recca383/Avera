using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Domain.Application.CaseImages;
using Avera.Domain.Application.Cases;
using Avera.Domain.Application.ExportedReports;
using Avera.Domain.Application.Notifications;
using Avera.Domain.Application.OverlayImages;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.TenantSubscriptions;
using Avera.Domain.Identity.Users;
using Infrastructure.DomainEvents;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Avera.Infrastructure.Database.Application
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
     IDomainEventsDispatcher domainEventsDispatcher,
     IUserContext userContext,
     UserManager<User> userManager)
    : DbContext(options), IApplicationDbContext
    {
        public DbSet<Case> Cases { get; set; }
        public DbSet<CaseImage> CaseImages { get; set; }
        public DbSet<GradCamImage> GradCamImages { get; set; }
        public DbSet<ExportedReport> ExportedReports { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly,
            type => type.Namespace == "Avera.Infrastructure.Database.Application.Configurations");

            //modelBuilder.HasDefaultSchema(Schemas.Default);

            modelBuilder.Entity<Case>()
            .HasQueryFilter(c =>
                c.TenantId == userContext.TenantId &&
                (
                    !userContext.IsUser ||
                    c.CreatedByUserId == userContext.UserId
                ));

            modelBuilder.Entity<CaseImage>()
                .HasQueryFilter(c =>
                c.Case.TenantId == userContext.TenantId &&
                (
                    !userContext.IsUser ||
                    c.Case.CreatedByUserId == userContext.UserId
                ));

            modelBuilder.Entity<GradCamImage>()
                .HasQueryFilter(c =>
                c.Case.TenantId == userContext.TenantId &&
                (
                    !userContext.IsUser ||
                    c.Case.CreatedByUserId == userContext.UserId
                ));

            modelBuilder.Entity<ExportedReport>()
                .HasQueryFilter(c =>
                c.Case.TenantId == userContext.TenantId &&
                (
                    !userContext.IsUser ||
                    c.Case.CreatedByUserId == userContext.UserId
                ));

            modelBuilder.Ignore<User>();
            modelBuilder.Ignore<Tenant>();
            modelBuilder.Ignore<TenantSubscription>();

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
