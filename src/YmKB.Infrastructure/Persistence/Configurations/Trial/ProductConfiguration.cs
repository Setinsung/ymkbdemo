using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YmKB.Domain.Entities.Trial;

namespace YmKB.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the Product entity.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    /// <summary>
    /// Configures the properties and relationships of the Product entity.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the Product entity.</param>
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Configures the Category property to be stored as a string in the database.
        builder.Property(x => x.Category).HasConversion<string>();

        // Configures the Name property to have a unique index.
        builder.HasIndex(x => x.Name).IsUnique();

        // Configures the Name property to have a maximum length of 80 characters and to be required.
        builder.Property(x => x.Name).HasMaxLength(80).IsRequired();

        // Ignores the DomainEvents property so it is not mapped to the database.
        builder.Ignore(e => e.DomainEvents);
    }
}
