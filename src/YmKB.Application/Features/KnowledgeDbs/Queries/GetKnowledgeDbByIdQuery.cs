using AutoMapper;
using Mediator;
using Microsoft.EntityFrameworkCore;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.KnowledgeDbs.DTOs;

namespace YmKB.Application.Features.KnowledgeDbs.Queries;

public record GetKnowledgeDbByIdQuery(string Id) : IFusionCacheRequest<KnowledgeDbDto?>
{
    public string CacheKey => $"KnowledgeDb_{Id}";

    public IEnumerable<string>? Tags => ["KnowledgeDbs"];
}

public class GetKnowledgeDbByIdQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetKnowledgeDbByIdQuery, KnowledgeDbDto?>
{
    public async ValueTask<KnowledgeDbDto?> Handle(
        GetKnowledgeDbByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var find = await dbContext
            .KnowledgeDbs
            .Where(p => p.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (find == null)
        {
            throw new KeyNotFoundException($"KnowledgeDb with Id '{request.Id}' was not found."); // Handle not found case
        }
        var data = mapper.Map<KnowledgeDbDto>(find);

        return data;
    }
}
