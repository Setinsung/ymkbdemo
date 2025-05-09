using AutoMapper;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.KbApps.EventHandlers;
using YmKB.Application.Features.KbDocFiles.Commands;
using YmKB.Application.Pipeline;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.KbApps.Commands;

public record DeleteKbAppCommand(params IEnumerable<string> Ids)
    : IFusionCacheRefreshRequest<Unit>,
        IRequiresValidation
{
    public IEnumerable<string>? Tags => [ "KbApps" ];
}

public class DeleteKbAppCommandHandler(
    ILogger<DeleteKbAppCommandHandler> logger,
    IApplicationDbContext dbContext,
    IMediator mediator
) : IRequestHandler<DeleteKbAppCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteKbAppCommand request,
        CancellationToken cancellationToken
    )
    {
        var toDeletes = dbContext.KbApps.Where(p => request.Ids.Contains(p.Id));

        foreach (var item in toDeletes)
        {
            item.AddDomainEvent(new KbAppDeletedEvent(item));
            dbContext.KbApps.Remove(item); // todo: executedelete
        }
        await dbContext.SaveChangesAsync(cancellationToken);

        // 知识库删除后，其下所有知识文档、向量全部删除
        var kbDocFileIds = await dbContext.KbDocFiles
            .Where(p => request.Ids.Contains(p.KbId))
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);
        await mediator.Send(new DeleteKbDocFileCommand(kbDocFileIds));
        return Unit.Value;
    }
}
