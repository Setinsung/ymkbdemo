using AutoMapper;
using Mediator;
using Microsoft.EntityFrameworkCore;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.KbDocFiles.DTOs;

namespace YmKB.Application.Features.KbDocFiles.Queries;

public record GetAllKbDocFilesQuery : IRequest<List<KbDocFileDto>>
{
    // public string CacheKey => "getallKbDocFiles";
    // public IEnumerable<string>? Tags => ["KbDocFiles"];
}

public class GetAllKbDocFilesQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetAllKbDocFilesQuery, List<KbDocFileDto>>
{
    public async ValueTask<List<KbDocFileDto>> Handle(
        GetAllKbDocFilesQuery request,
        CancellationToken cancellationToken
    )
    {
        var finds = await dbContext
            .KbDocFiles
            .OrderBy(x => x.Created)
            .ToListAsync(cancellationToken);
        var data = mapper.Map<List<KbDocFileDto>>(finds);
        return data;
    }
}
