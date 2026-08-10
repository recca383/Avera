using Avera.Domain.Identity.Tenants;
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
        }
    }
}