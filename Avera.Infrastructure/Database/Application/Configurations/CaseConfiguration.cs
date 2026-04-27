using Avera.Domain.Application.CaseImages;
using Avera.Domain.Application.Cases;
using Avera.Domain.Application.ExportedReports;
using Avera.Domain.Identity.Users;
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
            builder.HasIndex(c => c.CaseCode);

            builder.HasMany<CaseImage>(c => c.CaseImages)
                .WithOne(ci => ci.Case)
                .HasForeignKey(ci => ci.CaseId);

            builder.HasMany<ExportedReport>(c => c.ExportedReports)
                .WithOne(er => er.Case)
                .HasForeignKey(er => er.CaseId);
        }
    }
}
