using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YmKB.Domain.Entities;

namespace YmKB.Infrastructure.Persistence.Configurations;

public class QuantizedListConfig : IEntityTypeConfiguration<QuantizedList>
{
    public void Configure(EntityTypeBuilder<QuantizedList> builder)
    {
        builder.Property(x => x.Status).HasConversion<string>();
        builder.Ignore(e => e.DomainEvents);
    }
}