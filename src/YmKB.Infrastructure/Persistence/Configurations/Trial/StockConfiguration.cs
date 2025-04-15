using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YmKB.Domain.Entities.Trial;

namespace YmKB.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the Stock entity.
/// </summary>
public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    /// <summary>
    /// Configures the properties and relationships of the Stock entity.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the Stock entity.</param>
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        // Configures the ProductId property of the Stock entity.
        builder.Property(x => x.ProductId).HasMaxLength(50).IsRequired();

        // Configures the relationship between the Stock and Product entities.
        builder
            .HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configures the Location property of the Stock entity.
        builder.Property(x => x.Location).HasMaxLength(12).IsRequired();

        // Ignores the DomainEvents property of the Stock entity.
        builder.Ignore(e => e.DomainEvents);
    }
}
