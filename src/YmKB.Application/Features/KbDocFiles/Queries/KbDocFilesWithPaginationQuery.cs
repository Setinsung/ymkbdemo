using AutoMapper;
using Mediator;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Extensions;
using YmKB.Application.Features.KbDocFiles.DTOs;
using YmKB.Application.Models;

namespace YmKB.Application.Features.KbDocFiles.Queries;

public record KbDocFilesWithPaginationQuery(
    string? Keywords,
    string? KbId,
    int PageNumber = 0,
    int PageSize = 9,
    string OrderBy = "Id",
    string SortDirection = "Descending"
) : IRequest<PaginatedResult<KbDocFileDto>>
{
    // public string CacheKey =>
    //     $"KbDocFileswithpagination_{Keywords}_{KbId}_{PageNumber}_{PageSize}_{OrderBy}_{SortDirection}";
    // public IEnumerable<string>? Tags => [ "KbDocFiles" ];
}

public class KbDocFilesWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<KbDocFilesWithPaginationQuery, PaginatedResult<KbDocFileDto>>
{
    public async ValueTask<PaginatedResult<KbDocFileDto>> Handle(
        KbDocFilesWithPaginationQuery request,
        CancellationToken cancellationToken
    )
    {
        var data = await context
            .KbDocFiles
            .Where(e => request.KbId == null || e.KbId == request.KbId)
            // .Where(e => e.Type != "web")
            .OrderBy(request.OrderBy, request.SortDirection) // Dynamic ordering
            .ProjectToPaginatedDataAsync(
                condition: x =>
                    string.IsNullOrWhiteSpace(request.Keywords)
                    || x.FileName.Contains(request.Keywords),
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                mapperFunc: mapper.Map<KbDocFileDto>,
                cancellationToken: cancellationToken
            );
        return data;
    }
}
