using AutoMapper;
using Mediator;
using Microsoft.EntityFrameworkCore;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.AIModels.DTOs;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.AIModels.Queries;

public record SearchAIModelsQuery(string? SearchKeyword, AIModelType? AIModelType = null)
    : IFusionCacheRequest<List<AIModelDto>>
{
    public string CacheKey => $"searchallAIModels_{SearchKeyword}_{AIModelType}";
    public IEnumerable<string>? Tags => [ "AIModels" ];
}

public class SearchAIModelsQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<SearchAIModelsQuery, List<AIModelDto>>
{
    public async ValueTask<List<AIModelDto>> Handle(
        SearchAIModelsQuery request,
        CancellationToken cancellationToken
    )
    {
        var query = dbContext.AIModels.AsQueryable();
        if (request.AIModelType != null)
        {
            query = query.Where(x => x.AIModelType == request.AIModelType);
        }

        if (!string.IsNullOrEmpty(request.SearchKeyword))
        {
            query = query.Where(
                x =>
                    x.ModelName.Contains(request.SearchKeyword)
                    || x.ModelDescription.Contains(request.SearchKeyword)
            );
        }

        var finds = await query.OrderBy(x => x.Created).ToListAsync(cancellationToken);

        var data = mapper.Map<List<AIModelDto>>(finds);
        return data;
    }
}
