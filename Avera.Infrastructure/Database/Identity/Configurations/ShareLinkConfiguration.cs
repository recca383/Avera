using Avera.Infrastructure.Identity.ShareLinks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avera.Infrastructure.Database.Identity.Configurations
{
    internal class ShareLinkConfiguration : IEntityTypeConfiguration<ShareLink>
    {
        public void Configure(EntityTypeBuilder<ShareLink> builder)
        {
            builder.HasKey(sl => sl.Id);

            builder.HasIndex(sl => sl.Id);
            builder.HasIndex(sl => sl.CreatedAt);
            builder.HasIndex(sl => sl.TenantId);

        }
    }
}