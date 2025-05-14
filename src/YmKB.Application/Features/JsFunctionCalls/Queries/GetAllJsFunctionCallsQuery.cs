using AutoMapper;
using Mediator;
using Microsoft.EntityFrameworkCore;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.JsFunctionCalls.DTOs;

namespace YmKB.Application.Features.JsFunctionCalls.Queries;

public record GetAllJsFunctionCallsQuery : IFusionCacheRequest<List<JsFunctionCallDto>>
{
    public string CacheKey => "getallJsFunctionCalls";
    public IEnumerable<string>? Tags => ["JsFunctionCalls"];
}

public class GetAllJsFunctionCallsQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetAllJsFunctionCallsQuery, List<JsFunctionCallDto>>
{
    public async ValueTask<List<JsFunctionCallDto>> Handle(
        GetAllJsFunctionCallsQuery request,
        CancellationToken cancellationToken
    )
    {
        var finds = await dbContext
            .JsFunctionCalls
            .OrderBy(x => x.Created)
            .ToListAsync(cancellationToken);
        var data = mapper.Map<List<JsFunctionCallDto>>(finds);
        return data;
    }
}
