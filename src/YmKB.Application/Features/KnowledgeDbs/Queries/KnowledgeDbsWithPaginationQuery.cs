using AutoMapper;
using Mediator;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Extensions;
using YmKB.Application.Features.KnowledgeDbs.DTOs;
using YmKB.Application.Models;

namespace YmKB.Application.Features.KnowledgeDbs.Queries;

public record KnowledgeDbsWithPaginationQuery(
    string? Keywords,
    int PageNumber = 0,
    int PageSize = 9,
    string OrderBy = "Id",
    string SortDirection = "Descending"
) : IFusionCacheRequest<PaginatedResult<KnowledgeDbDto>>
{
    public string CacheKey =>
        $"KnowledgeDbswithpagination_{Keywords}_{PageNumber}_{PageSize}_{OrderBy}_{SortDirection}";
    public IEnumerable<string>? Tags => [ "KnowledgeDbs" ];
}

public class KnowledgeDbsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<KnowledgeDbsWithPaginationQuery, PaginatedResult<KnowledgeDbDto>>
{
    public async ValueTask<PaginatedResult<KnowledgeDbDto>> Handle(
        KnowledgeDbsWithPaginationQuery request,
        CancellationToken cancellationToken
    )
    {
        var data = await context
            .KnowledgeDbs
            .OrderBy(request.OrderBy, request.SortDirection) // Dynamic ordering
            .ProjectToPaginatedDataAsync(
                condition: x =>
                    string.IsNullOrWhiteSpace(request.Keywords)
                    || x.Name.Contains(request.Keywords)
                    || x.Description.Contains(request.Keywords),
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                mapperFunc: mapper.Map<KnowledgeDbDto>,
                cancellationToken: cancellationToken
            );
        return data;
    }
}
