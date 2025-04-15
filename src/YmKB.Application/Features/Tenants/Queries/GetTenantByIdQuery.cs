using Mediator;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.Tenants.DTOs;

namespace YmKB.Application.Features.Tenants.Queries;

public record GetTenantByIdQuery(string Id) : IFusionCacheRequest<TenantDto?>
{
    public string CacheKey => $"Tenant_{Id}";
    public IEnumerable<string>? Tags => [ "tenants" ];
}

public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, TenantDto?>
{
    private readonly IApplicationDbContext _dbContext;

    public GetTenantByIdQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<TenantDto?> Handle(
        GetTenantByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var tenant = await _dbContext.Tenants.FindAsync([ request.Id ], cancellationToken);
        if (tenant == null)
        {
            return null; // or throw an exception if preferred
        }
        return new TenantDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Description = tenant.Description
        };
    }
}
