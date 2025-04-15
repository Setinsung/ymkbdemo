using Mediator;
using Microsoft.Extensions.Logging;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Domain.Abstractions.SystemEntities;

namespace YmKB.Application.Features.Tenants.Commainds;

public record CreateTenantCommand(string Name, string Description)
    : IFusionCacheRefreshRequest<string>
{
    public IEnumerable<string>? Tags => [ "tenants" ];
}

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, string>
{
    private readonly ILogger<CreateTenantCommandHandler> _logger;
    private readonly IApplicationDbContext _dbContext;

    public CreateTenantCommandHandler(
        ILogger<CreateTenantCommandHandler> logger,
        IApplicationDbContext dbContext
    )
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async ValueTask<string> Handle(
        CreateTenantCommand request,
        CancellationToken cancellationToken
    )
    {
        // 创建具有唯一 ID 的新租户实例
        var tenant = new Tenant { Name = request.Name, Description = request.Description };
        // 将租户添加到数据库的逻辑
        _dbContext.Tenants.Add(tenant);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Tenant {TenantId} is created", tenant.Id);
        return tenant.Id;
    }
}
