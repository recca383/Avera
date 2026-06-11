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
using Avera.Domain.Application.OverlayImages;
using Avera.Infrastructure.Identity.ShareLinks;
using Avera.Infrastructure.Identity.Tenants;
using Avera.Infrastructure.Identity.TenantSubscriptions;
using Avera.Infrastructure.Identity.Users;
using Infrastructure.Database;
using Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using SharedKernel;

namespace Avera.Infrastructure.Database.Application
{
    internal class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
     IDomainEventsDispatcher domainEventsDispatcher)
    : DbContext(options), IApplicationDbContext
    {
        public DbSet<Case> Cases { get; set; }
        public DbSet<CaseImage> CaseImages { get; set; }
        public DbSet<GradCamImage> GradCamImages { get; set; }
        public DbSet<ExportedReport> ExportedReports { get; set; }
        public DbSet<ShareLink> ShareLinks { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly,
            type => type.Namespace == "Avera.Infrastructure.Database.Application.Configurations");
            
            //modelBuilder.HasDefaultSchema(Schemas.Default);

            modelBuilder.Ignore<User>();
            modelBuilder.Ignore<ShareLink>();
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
