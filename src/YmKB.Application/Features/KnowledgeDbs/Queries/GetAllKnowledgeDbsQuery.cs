using AutoMapper;
using Mediator;
using Microsoft.EntityFrameworkCore;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.KnowledgeDbs.DTOs;

namespace YmKB.Application.Features.KnowledgeDbs.Queries;

public record GetAllKnowledgeDbsQuery : IFusionCacheRequest<List<KnowledgeDbDto>>
{
    public string CacheKey => "getallKnowledgeDbs";
    public IEnumerable<string>? Tags => ["KnowledgeDbs"];
}

public class GetAllKnowledgeDbsQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetAllKnowledgeDbsQuery, List<KnowledgeDbDto>>
{
    public async ValueTask<List<KnowledgeDbDto>> Handle(
        GetAllKnowledgeDbsQuery request,
        CancellationToken cancellationToken
    )
    {
        var finds = await dbContext
            .KnowledgeDbs
            .OrderBy(x => x.Created)
            .ToListAsync(cancellationToken);
        var data = mapper.Map<List<KnowledgeDbDto>>(finds);
        return data;
    }
}
