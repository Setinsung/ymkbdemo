using Microsoft.EntityFrameworkCore;
using YmKB.Domain.Abstractions.SystemEntities;
using YmKB.Domain.Entities.Trial;

namespace YmKB.Application.Contracts.Persistence;

/// <summary>
/// 定义应用程序数据库上下文的接口。
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Product> Products { get; set; }

    DbSet<Stock> Stocks { get; set; }

    DbSet<AuditTrail> AuditTrails { get; set; }

    DbSet<Tenant> Tenants { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
