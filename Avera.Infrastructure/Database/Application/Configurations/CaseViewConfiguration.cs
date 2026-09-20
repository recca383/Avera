using Avera.Domain.Application.CaseViews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avera.Infrastructure.Database.Application.Configurations;

internal sealed class CaseViewConfiguration : IEntityTypeConfiguration<CaseView>
{
    public void Configure(EntityTypeBuilder<CaseView> builder)
    {
        builder.HasKey(cv => new { cv.CaseId, cv.UserId });

        builder.Property(cv => cv.ViewedAt).IsRequired();

        builder.HasIndex(cv => new { cv.CaseId, cv.UserId }).IsUnique();
    }
}
