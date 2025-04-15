using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YmKB.Domain.Entities;

namespace YmKB.Infrastructure.Persistence.Configurations;

public class KnowledgeDbConfig : IEntityTypeConfiguration<KnowledgeDb>
{
    public void Configure(EntityTypeBuilder<KnowledgeDb> builder)
    {
        builder.Ignore(e => e.DomainEvents);    
    }
}