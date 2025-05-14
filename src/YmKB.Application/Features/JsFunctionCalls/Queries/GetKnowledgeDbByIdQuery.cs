using AutoMapper;
using Mediator;
using Microsoft.EntityFrameworkCore;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.JsFunctionCalls.DTOs;

namespace YmKB.Application.Features.JsFunctionCalls.Queries;

public record GetJsFunctionCallByIdQuery(string Id) : IFusionCacheRequest<JsFunctionCallDto?>
{
    public string CacheKey => $"JsFunctionCall_{Id}";

    public IEnumerable<string>? Tags => ["JsFunctionCalls"];
}

public class GetJsFunctionCallByIdQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetJsFunctionCallByIdQuery, JsFunctionCallDto?>
{
    public async ValueTask<JsFunctionCallDto?> Handle(
        GetJsFunctionCallByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var find = await dbContext
            .JsFunctionCalls
            .Where(p => p.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (find == null)
        {
            throw new KeyNotFoundException($"JsFunctionCall with Id '{request.Id}' was not found."); // Handle not found case
        }
        var data = mapper.Map<JsFunctionCallDto>(find);

        return data;
    }
}
