using AutoMapper;
using Mediator;
using Microsoft.EntityFrameworkCore;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.AIModels.DTOs;

namespace YmKB.Application.Features.AIModels.Queries;

public record GetAIModelByIdQuery(string Id) : IFusionCacheRequest<AIModelDto?>
{
    public string CacheKey => $"AIModel_{Id}";

    public IEnumerable<string>? Tags => ["AIModels"];
}

public class GetAIModelByIdQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetAIModelByIdQuery, AIModelDto?>
{
    public async ValueTask<AIModelDto?> Handle(
        GetAIModelByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var find = await dbContext
            .AIModels
            .Where(p => p.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (find == null)
        {
            throw new KeyNotFoundException($"AIModel with Id '{request.Id}' was not found."); // Handle not found case
        }
        var data = mapper.Map<AIModelDto>(find);

        return data;
    }
}
