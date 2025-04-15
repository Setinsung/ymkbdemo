using Mediator;
using Microsoft.Extensions.Logging;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;

namespace YmKB.Application.Features.Tenants.Commainds;

public record UpdateTenantCommand(string Id, string Name, string Description) : IFusionCacheRefreshRequest<Unit>
{
    public IEnumerable<string>? Tags => ["tenants"];
}


public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand,Unit>
{
    private readonly ILogger<UpdateTenantCommandHandler> _logger;
    private readonly IApplicationDbContext _dbContext;

    public UpdateTenantCommandHandler(ILogger<UpdateTenantCommandHandler> logger, IApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async ValueTask<Unit> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        // 用于更新数据库中的租户的逻辑
        var tenant = await _dbContext.Tenants.FindAsync([request.Id], cancellationToken);
        if (tenant == null)
        {
            _logger.LogError($"Tenant with Id {request.Id} not found.");
            throw new Exception($"Tenant with Id {request.Id} not found.");
        }

        tenant.Name = request.Name;
        tenant.Description = request.Description;
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Updated Tenant: {request.Id}, Name: {request.Name}");
        return Unit.Value;
    }
}
