using AutoMapper;
using Mediator;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Extensions;
using YmKB.Application.Features.JsFunctionCalls.DTOs;
using YmKB.Application.Models;

namespace YmKB.Application.Features.JsFunctionCalls.Queries;

public record JsFunctionCallsWithPaginationQuery(
    string? Keywords,
    int PageNumber = 0,
    int PageSize = 9,
    string OrderBy = "Id",
    string SortDirection = "Descending"
) : IFusionCacheRequest<PaginatedResult<JsFunctionCallDto>>
{
    public string CacheKey =>
        $"JsFunctionCallswithpagination_{Keywords}_{PageNumber}_{PageSize}_{OrderBy}_{SortDirection}";
    public IEnumerable<string>? Tags => [ "JsFunctionCalls" ];
}

public class JsFunctionCallsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<JsFunctionCallsWithPaginationQuery, PaginatedResult<JsFunctionCallDto>>
{
    public async ValueTask<PaginatedResult<JsFunctionCallDto>> Handle(
        JsFunctionCallsWithPaginationQuery request,
        CancellationToken cancellationToken
    )
    {
        var data = await context
            .JsFunctionCalls
            .OrderBy(request.OrderBy, request.SortDirection) // Dynamic ordering
            .ProjectToPaginatedDataAsync(
                condition: x =>
                    string.IsNullOrWhiteSpace(request.Keywords)
                    || x.Name.Contains(request.Keywords)
                    || x.Description.Contains(request.Keywords),
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                mapperFunc: mapper.Map<JsFunctionCallDto>,
                cancellationToken: cancellationToken
            );
        return data;
    }
}
