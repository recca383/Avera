using Avera.Domain.Application.Cases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avera.Infrastructure.Database.Application.Configurations
{
    internal class GradCamImageConfiguration : IEntityTypeConfiguration<Domain.Application.OverlayImages.GradCamImage>
    {
        public void Configure(EntityTypeBuilder<Domain.Application.OverlayImages.GradCamImage> builder)
        {
            builder.HasKey(oi => oi.Id);

            builder.HasIndex(oi => oi.CaseId);

            builder.HasOne<Case>(ci => ci.Case)
                .WithMany(c => c.GradCamImages)
                .HasForeignKey(ci => ci.CaseId);

            builder.Property(oi => oi.Slot)
                .HasConversion<string>();

            builder.Property(oi => oi.Type)
                .HasConversion<string>();

            builder.Property(oi => oi.BlobPath)
                .IsRequired();
        }
    }
}