using Avera.Domain.Application.ExportedReports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avera.Infrastructure.Database.Application.Configurations
{
    internal class ExportedReportConfiguration : IEntityTypeConfiguration<ExportedReport>
    {
        public void Configure(EntityTypeBuilder<ExportedReport> builder)
        {
            builder.HasKey(er => er.Id);

            builder.HasIndex(er => er.CaseId);
            builder.HasIndex(er => er.CreatedAt);

            builder.HasOne(er => er.Case)
                .WithMany(c => c.ExportedReports)
                .HasForeignKey(er => er.CaseId)
                .IsRequired();
        }
    }
}