using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YmKB.Domain.Entities;

namespace YmKB.Infrastructure.Persistence.Configurations;

public class OnlyChatHistoryConfig : IEntityTypeConfiguration<OnlyChatHistory>
{
    public void Configure(EntityTypeBuilder<OnlyChatHistory> builder)
    {
        builder.Property(x => x.MessageType).HasConversion<string>();
        builder.Ignore(e => e.DomainEvents);    
    }
}