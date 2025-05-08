using AutoMapper;
using Mediator;
using Microsoft.EntityFrameworkCore;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.QuantizedLists.DTOs;

namespace YmKB.Application.Features.QuantizedLists.Queries;

public record GetAllQuantizedListsQuery : IRequest<List<QuantizedListDto>>
{
    // public string CacheKey => "getallQuantizedLists";
    // public IEnumerable<string>? Tags => ["QuantizedLists"];
}

public class GetAllQuantizedListsQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetAllQuantizedListsQuery, List<QuantizedListDto>>
{
    public async ValueTask<List<QuantizedListDto>> Handle(
        GetAllQuantizedListsQuery request,
        CancellationToken cancellationToken
    )
    {
        var finds = await dbContext
            .QuantizedLists
            .OrderBy(x => x.Created)
            .Select(x => new QuantizedListDto
                {
                    Id = x.Id,
                    KbId = x.KbId,
                    FileName = dbContext.KbDocFiles.FirstOrDefault(y => y.Id == x.KbDocFileId).FileName ?? "",
                    KbDocFileId = x.KbDocFileId,
                    Status = x.Status,
                    Remark = x.Remark
                })
            .ToListAsync(cancellationToken);
        return finds;
    }
}
