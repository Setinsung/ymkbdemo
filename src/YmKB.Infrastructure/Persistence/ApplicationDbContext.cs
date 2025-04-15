using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YmKB.Application.Contracts.Persistence;
using YmKB.Domain.Abstractions.Common;
using YmKB.Domain.Abstractions.Identities;
using YmKB.Domain.Abstractions.SystemEntities;
using YmKB.Domain.Entities;
using YmKB.Domain.Entities.Trial;
using YmKB.Infrastructure.Persistence.Extensions;

namespace YmKB.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options), IApplicationDbContext
{
    public DbSet<AIModel> AIModels { get; set; }
    public DbSet<KbApp> KbApps { get; set; }
    public DbSet<KbDocFile> KbDocFiles { get; set; }
    public DbSet<KnowledgeDb> KnowledgeDbs { get; set; }
    public DbSet<OnlyChatHistory> OnlyChatHistories { get; set; }
    public DbSet<QuantizedList> QuantizedLists { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<AuditTrail> AuditTrails { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.ApplyGlobalFilters<ISoftDelete>(e => e.Deleted == null);
    }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<string>().HaveMaxLength(450);
    }
}