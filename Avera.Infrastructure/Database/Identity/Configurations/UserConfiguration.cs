using Avera.Domain.Application.Cases;
using Avera.Domain.Identity.Users;
using Avera.Infrastructure.Identity;
using Avera.Infrastructure.Identity.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avera.Infrastructure.Database.Identity.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.HasIndex(u => u.Id);
            builder.HasIndex(u => u.TenantId);
            
        }
    }
}