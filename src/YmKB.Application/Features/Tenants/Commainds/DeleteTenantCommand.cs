using Mediator;
using Microsoft.Extensions.Logging;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;

namespace YmKB.Application.Features.Tenants.Commainds;

public record DeleteTenantCommand(params IEnumerable<string> Ids) : IFusionCacheRefreshRequest<Unit>
{
    public IEnumerable<string>? Tags => ["tenants"];
}



public class DeleteTenantCommandHandler : IRequestHandler<DeleteTenantCommand>
{
    private readonly ILogger<DeleteTenantCommandHandler> _logger;
    private readonly IApplicationDbContext _dbContext;

    public DeleteTenantCommandHandler(ILogger<DeleteTenantCommandHandler> logger, IApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async ValueTask<Unit> Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
    {
        // 从数据库中删除租户的逻辑
        _dbContext.Tenants.RemoveRange(_dbContext.Tenants.Where(t => request.Ids.Contains(t.Id)));
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Tenants with Ids {TenantIds} are deleted", string.Join(", ", request.Ids));
        return Unit.Value;
    }
}
