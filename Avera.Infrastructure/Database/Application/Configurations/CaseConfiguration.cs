using Avera.Domain.Application.CaseImages;
using Avera.Domain.Application.Cases;
using Avera.Domain.Application.CaseViews;
using Avera.Domain.Application.ExportedReports;
using Avera.Domain.Cases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avera.Infrastructure.Database.Application.Configurations
{
    internal sealed class CaseConfiguration : IEntityTypeConfiguration<Case>
    {
        public void Configure(EntityTypeBuilder<Case> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasIndex(c => c.Id);
            builder.HasIndex(c => c.CreatedAt);
            builder.HasIndex(c => c.CaseCode).IsUnique();

            builder.HasMany<CaseImage>(c => c.CaseImages)
                .WithOne(ci => ci.Case)
                .HasForeignKey(ci => ci.CaseId);

            builder.HasMany<ExportedReport>(c => c.ExportedReports)
                .WithOne(er => er.Case)
                .HasForeignKey(er => er.CaseId);

            builder.OwnsOne<MLResponse>(c => c.MLResponse, ml =>
            {
                ml.Property(x => x.ConfidenceForged);
                ml.Property(x => x.ConfidenceGenuine);
                ml.Property(x => x.Distance);
                ml.Property(x => x.Threshold);
                ml.Property(x => x.Verdict);
            });

            builder.HasMany<CaseView>(c => c.CaseViews)
                .WithOne(cv => cv.Case)
                .HasForeignKey(cv => cv.CaseId);
            
        }
    }
}
