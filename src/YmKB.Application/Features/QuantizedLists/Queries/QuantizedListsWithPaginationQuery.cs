using AutoMapper;
using Mediator;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Extensions;
using YmKB.Application.Features.QuantizedLists.DTOs;
using YmKB.Application.Models;

namespace YmKB.Application.Features.QuantizedLists.Queries;

public record QuantizedListsWithPaginationQuery(
    string? Keywords,
    string? KbId,
    int PageNumber = 0,
    int PageSize = 9,
    string OrderBy = "Id",
    string SortDirection = "Descending"
) : IRequest<PaginatedResult<QuantizedListDto>>
{
    // public string CacheKey =>
    //     $"QuantizedListswithpagination_{Keywords}_{KbId}_{PageNumber}_{PageSize}_{OrderBy}_{SortDirection}";
    // public IEnumerable<string>? Tags => [ "QuantizedLists" ];
}

public class QuantizedListsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<QuantizedListsWithPaginationQuery, PaginatedResult<QuantizedListDto>>
{
    public async ValueTask<PaginatedResult<QuantizedListDto>> Handle(
        QuantizedListsWithPaginationQuery request,
        CancellationToken cancellationToken
    )
    {
        var data = await context
            .QuantizedLists
            .Where(e => request.KbId == null || e.KbId == request.KbId)
            .Select(x => new QuantizedListDto()
            {
                Id = x.Id,
                KbId = x.KbId,
                FileName = context.KbDocFiles.FirstOrDefault(y => y.Id == x.KbDocFileId).FileName?? "",
                KbDocFileId = x.KbDocFileId,
                Status = x.Status,
                Remark = x.Remark
            })
            .OrderBy(request.OrderBy, request.SortDirection)
            .ProjectToPaginatedDataAsync(
                condition: x =>
                    string.IsNullOrWhiteSpace(request.Keywords)
                    || x.FileName.Contains(request.Keywords),
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                mapperFunc: e => e,
                cancellationToken: cancellationToken
            );
        return data;
    }
}
