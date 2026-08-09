using Avera.Domain.Identity.MemberRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avera.Infrastructure.Database.Identity.Configurations
{
    internal sealed class MemberRequestConfiguration : IEntityTypeConfiguration<MemberRequest>
    {
        public void Configure(EntityTypeBuilder<MemberRequest> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.TenantId);
            builder.HasIndex(x => x.UserId);
             builder.HasIndex(x => new
            {
                x.TenantId,
                x.UserId,
                x.Status
            });

            builder.HasOne(x => x.Tenant)
                .WithMany(x => x.MemberRequests)
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}