using AutoMapper;
using Mediator;
using Microsoft.EntityFrameworkCore;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.QuantizedLists.DTOs;

namespace YmKB.Application.Features.QuantizedLists.Queries;

public record GetQuantizedListByIdQuery(string Id) : IRequest<QuantizedListDto?>
{
    // public string CacheKey => $"QuantizedList_{Id}";
    //
    // public IEnumerable<string>? Tags => ["QuantizedLists"];
}

public class GetQuantizedListByIdQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetQuantizedListByIdQuery, QuantizedListDto?>
{
    public async ValueTask<QuantizedListDto?> Handle(
        GetQuantizedListByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var find = await dbContext
            .QuantizedLists
            .Where(p => p.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);
        var kbDocFile = await dbContext.KbDocFiles.FirstOrDefaultAsync(y => y.Id == find.KbDocFileId, cancellationToken);
        if (find == null)
        {
            throw new KeyNotFoundException($"QuantizedList with Id '{request.Id}' was not found."); // Handle not found case
        }
        var data = mapper.Map<QuantizedListDto>(find);
        data.FileName = kbDocFile.FileName;
        return data;
    }
}
