using AutoMapper;
using Mediator;
using Microsoft.EntityFrameworkCore;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.KbApps.DTOs;

namespace YmKB.Application.Features.KbApps.Queries;

public record GetKbAppByIdQuery(string Id) : IFusionCacheRequest<KbAppDto?>
{
    public string CacheKey => $"KbApp_{Id}";

    public IEnumerable<string>? Tags => [ "KbApps" ];
}

public class GetKbAppByIdQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetKbAppByIdQuery, KbAppDto?>
{
    public async ValueTask<KbAppDto?> Handle(
        GetKbAppByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var find = await dbContext
            .KbApps
            .Where(p => p.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (find == null)
        {
            throw new KeyNotFoundException($"KbApp with Id '{request.Id}' was not found."); // Handle not found case
        }
        var data = mapper.Map<KbAppDto>(find);

        return data;
    }
}
