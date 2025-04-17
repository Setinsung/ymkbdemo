using Microsoft.EntityFrameworkCore;
using YmKB.Domain.Abstractions.SystemEntities;
using YmKB.Domain.Entities;
using YmKB.Domain.Entities.Trial;

namespace YmKB.Application.Contracts.Persistence;

/// <summary>
/// 定义应用程序数据库上下文的接口。
/// </summary>
public interface IApplicationDbContext
{
    DbSet<AIModel> AIModels { get; set; }
    DbSet<KbApp> KbApps { get; set; }
    DbSet<KbDocFile> KbDocFiles { get; set; }
    DbSet<KnowledgeDb> KnowledgeDbs { get; set; }
    DbSet<OnlyChatHistory> OnlyChatHistories { get; set; }
    DbSet<QuantizedList> QuantizedLists { get; set; }

    DbSet<JsFunctionCall> JsFunctionCalls { get; set; }

    # region system
    DbSet<AuditTrail> AuditTrails { get; set; }
    DbSet<Tenant> Tenants { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<Stock> Stocks { get; set; }
    # endregion
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
