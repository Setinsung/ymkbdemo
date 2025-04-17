using AutoMapper;
using Mediator;
using Microsoft.EntityFrameworkCore;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.KbApps.DTOs;

namespace YmKB.Application.Features.KbApps.Queries;

public record GetAllKbAppsQuery : IFusionCacheRequest<IEnumerable<KbAppDto>>
{
    public string CacheKey => "AllKbApps";

    public IEnumerable<string>? Tags => [ "KbApps" ];
}

public class GetAllKbAppsQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetAllKbAppsQuery, IEnumerable<KbAppDto>>
{
    public async ValueTask<IEnumerable<KbAppDto>> Handle(
        GetAllKbAppsQuery request,
        CancellationToken cancellationToken
    )
    {
        var items = await dbContext.KbApps.ToListAsync(cancellationToken);
        return mapper.Map<IEnumerable<KbAppDto>>(items);
    }
}
