using Avera.Infrastructure.Identity.InviteCodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avera.Infrastructure.Database.Identity.Configurations
{
    internal class ShareLinkConfiguration : IEntityTypeConfiguration<InviteCode>
    {
        public void Configure(EntityTypeBuilder<InviteCode> builder)
        {
            builder.HasKey(sl => sl.Id);

            builder.HasIndex(sl => sl.Id);
            builder.HasIndex(sl => sl.CreatedAt);
            builder.HasIndex(sl => sl.TenantId);

        }
    }
}