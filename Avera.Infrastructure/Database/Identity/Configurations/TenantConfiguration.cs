using Avera.Infrastructure.Identity.InviteCodes;
using Avera.Infrastructure.Identity.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avera.Infrastructure.Database.Identity.Configurations
{
    internal class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasIndex(t => t.Id);

            builder.HasMany<InviteCode>(t => t.ShareLinks)
                .WithOne(sl => sl.Tenant)
                .HasForeignKey(sl => sl.TenantId);
        }
    }
}