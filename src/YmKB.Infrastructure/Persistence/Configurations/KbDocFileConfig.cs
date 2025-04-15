using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YmKB.Domain.Entities;

namespace YmKB.Infrastructure.Persistence.Configurations;

public class KbDocFileConfig : IEntityTypeConfiguration<KbDocFile>
{
    public void Configure(EntityTypeBuilder<KbDocFile> builder)
    {
        builder.Property(x => x.Status).HasConversion<string>();
        builder.Ignore(e => e.DomainEvents);    
    }
}