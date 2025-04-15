using Mediator;
using Microsoft.EntityFrameworkCore;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.Tenants.DTOs;

namespace YmKB.Application.Features.Tenants.Queries;

public record GetAllTenantsQuery : IFusionCacheRequest<List<TenantDto>>
{
    public string CacheKey => "GetAllTenants";
    public IEnumerable<string>? Tags => [ "tenants" ];
}

public class GetAllTenantsQueryHandler : IRequestHandler<GetAllTenantsQuery, List<TenantDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetAllTenantsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<List<TenantDto>> Handle(
        GetAllTenantsQuery request,
        CancellationToken cancellationToken
    )
    {
        var tenants = await _dbContext
            .Tenants
            .OrderBy(x => x.Name)
            .Select(
                t =>
                    new TenantDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description
                    }
            )
            .ToListAsync(cancellationToken);

        return tenants;
    }
}
