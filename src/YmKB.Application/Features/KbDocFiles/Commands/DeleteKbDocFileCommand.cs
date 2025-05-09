using Mediator;
using Microsoft.Extensions.Logging;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.KbDocFiles.EventHandlers;
using YmKB.Application.Pipeline;

namespace YmKB.Application.Features.KbDocFiles.Commands;

public record DeleteKbDocFileCommand(params IEnumerable<string> Ids)
    : IFusionCacheRefreshRequest<Unit>,
        IRequiresValidation
{
    public IEnumerable<string>? Tags => [ "KbDocFiles" ];
}

public class DeleteKbDocFileCommandHandler(
    ILogger<DeleteKbDocFileCommandHandler> logger,
    IApplicationDbContext dbContext,
    IMediator mediator
) : IRequestHandler<DeleteKbDocFileCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteKbDocFileCommand request,
        CancellationToken cancellationToken
    )
    {
        var toDeletes = dbContext.KbDocFiles.Where(p => request.Ids.Contains(p.Id));

        foreach (var item in toDeletes)
        {
            item.AddDomainEvent(new KbDocFileDeletedEvent(item));
            dbContext.KbDocFiles.Remove(item); // todo: executedelete
        }
        await dbContext.SaveChangesAsync(cancellationToken);

        // 删除后同时删除向量
        try
        {
            foreach (var item in toDeletes)
            {
                await mediator.Send(new DeleteKbDocFileVectorCommand(item.Id));
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, $"删除{request.Ids}的文档时，同时删除向量失败");
        }

        return Unit.Value;
    }
}
