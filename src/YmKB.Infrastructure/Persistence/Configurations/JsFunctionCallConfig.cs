using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YmKB.Domain.Entities;
using YmKB.Infrastructure.Persistence.Conversions;

namespace YmKB.Infrastructure.Persistence.Configurations;

public class JsFunctionCallConfig : IEntityTypeConfiguration<JsFunctionCall>
{
    public void Configure(EntityTypeBuilder<JsFunctionCall> builder)
    {
        builder.Property(x => x.Parameters).HasJsonConversion();
        builder.Ignore(e => e.DomainEvents);
    }
}