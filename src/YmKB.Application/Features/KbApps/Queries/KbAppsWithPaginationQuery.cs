using AutoMapper;
using Mediator;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Extensions;
using YmKB.Application.Features.KbApps.DTOs;
using YmKB.Application.Models;

namespace YmKB.Application.Features.KbApps.Queries;

public record KbAppsWithPaginationQuery(
    string? Keywords,
    int PageNumber = 0,
    int PageSize = 9,
    string OrderBy = "Id",
    string SortDirection = "Descending"
) : IFusionCacheRequest<PaginatedResult<KbAppDto>>
{
    public string CacheKey =>
        $"KbAppswithpagination_{Keywords}_{PageNumber}_{PageSize}_{OrderBy}_{SortDirection}";
    public IEnumerable<string>? Tags => [ "KbApps" ];
}

public class KbAppsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<KbAppsWithPaginationQuery, PaginatedResult<KbAppDto>>
{
    public async ValueTask<PaginatedResult<KbAppDto>> Handle(
        KbAppsWithPaginationQuery request,
        CancellationToken cancellationToken
    )
    {
        var data = await context
            .KbApps
            .OrderBy(request.OrderBy, request.SortDirection) // Dynamic ordering
            .ProjectToPaginatedDataAsync(
                condition: x =>
                    string.IsNullOrWhiteSpace(request.Keywords)
                    || x.Name.Contains(request.Keywords)
                    || x.Description.Contains(request.Keywords),
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                mapperFunc: mapper.Map<KbAppDto>,
                cancellationToken: cancellationToken
            );
        return data;
    }
}
