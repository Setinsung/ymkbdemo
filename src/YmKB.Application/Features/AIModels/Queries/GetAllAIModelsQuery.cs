using AutoMapper;
using Mediator;
using Microsoft.EntityFrameworkCore;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.AIModels.DTOs;

namespace YmKB.Application.Features.AIModels.Queries;

public record GetAllAIModelsQuery : IFusionCacheRequest<List<AIModelDto>>
{
    public string CacheKey => "getallAIModels";
    public IEnumerable<string>? Tags => ["AIModels"];
}

public class GetAllAIModelsQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetAllAIModelsQuery, List<AIModelDto>>
{
    public async ValueTask<List<AIModelDto>> Handle(
        GetAllAIModelsQuery request,
        CancellationToken cancellationToken
    )
    {
        var finds = await dbContext
            .AIModels
            .OrderBy(x => x.Created)
            .ToListAsync(cancellationToken);
        var data = mapper.Map<List<AIModelDto>>(finds);
        return data;
    }
}
