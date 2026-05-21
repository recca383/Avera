using Avera.Domain.Application.CaseImages;
using Avera.Domain.Application.Cases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avera.Infrastructure.Database.Application.Configurations
{
    internal sealed class CaseConfigurations : IEntityTypeConfiguration<CaseImage>
    {
        public void Configure(EntityTypeBuilder<CaseImage> builder)
        {
            builder.HasKey(ci => ci.Id);
            
            builder.HasIndex(ci => ci.CaseId);
            builder.HasIndex(ci => ci.UploadedAt);
            
            builder.HasOne<Case>(ci => ci.Case)
                .WithMany(c => c.CaseImages)
                .HasForeignKey(ci => ci.CaseId);
        }
    }
}