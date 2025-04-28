using AutoMapper;
using Mediator;
using Microsoft.EntityFrameworkCore;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.KbDocFiles.DTOs;

namespace YmKB.Application.Features.KbDocFiles.Queries;

public record GetKbDocFileByIdQuery(string Id) : IFusionCacheRequest<KbDocFileDto?>
{
    public string CacheKey => $"KbDocFile_{Id}";

    public IEnumerable<string>? Tags => ["KbDocFiles"];
}

public class GetKbDocFileByIdQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetKbDocFileByIdQuery, KbDocFileDto?>
{
    public async ValueTask<KbDocFileDto?> Handle(
        GetKbDocFileByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var find = await dbContext
            .KbDocFiles
            .Where(p => p.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (find == null)
        {
            throw new KeyNotFoundException($"KbDocFile with Id '{request.Id}' was not found."); // Handle not found case
        }
        var data = mapper.Map<KbDocFileDto>(find);

        return data;
    }
}
