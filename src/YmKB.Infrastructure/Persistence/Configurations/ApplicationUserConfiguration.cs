using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YmKB.Domain.Abstractions.Identities;

namespace YmKB.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasOne(x => x.Superior).WithMany().HasForeignKey(u => u.SuperiorId);
        builder.Property(x => x.Nickname).HasMaxLength(50);
        builder.Property(x => x.Provider).HasMaxLength(50);
        builder.Property(x => x.TenantId).HasMaxLength(50);
        builder.Property(x => x.AvatarUrl).HasMaxLength(255);
        builder.Property(x => x.RefreshToken).HasMaxLength(255);
        builder.Property(x => x.LanguageCode).HasMaxLength(255);
        builder.Property(x => x.TimeZoneId).HasMaxLength(255);
        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.LastModifiedBy).HasMaxLength(50);
    }
}
