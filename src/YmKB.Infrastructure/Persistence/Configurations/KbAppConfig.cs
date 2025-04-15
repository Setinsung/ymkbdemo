using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YmKB.Domain.Entities;

namespace YmKB.Infrastructure.Persistence.Configurations;

public class KbAppConfig : IEntityTypeConfiguration<KbApp>
{
    public void Configure(EntityTypeBuilder<KbApp> builder)
    {
        builder.Property(x => x.KbAppType).HasConversion<string>();
        builder.Ignore(e => e.DomainEvents);
    }
}