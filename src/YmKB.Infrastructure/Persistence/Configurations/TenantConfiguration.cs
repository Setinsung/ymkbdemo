using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YmKB.Domain.Abstractions.SystemEntities;

namespace YmKB.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(x=>x.Name).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Id).HasMaxLength(36);
    }
}
