using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.KbDocFiles.Commands;
using YmKB.Application.Features.KnowledgeDbs.EventHandlers;
using YmKB.Application.Pipeline;

namespace YmKB.Application.Features.KnowledgeDbs.Commands;

public record DeleteKnowledgeDbCommand(params IEnumerable<string> Ids)
    : IFusionCacheRefreshRequest<Unit>,
        IRequiresValidation
{
    public IEnumerable<string>? Tags => [ "KnowledgeDbs" ];
}

public class DeleteKnowledgeDbCommandHandler(
    ILogger<DeleteKnowledgeDbCommandHandler> logger,
    IMediator mediator,
    IApplicationDbContext dbContext
) : IRequestHandler<DeleteKnowledgeDbCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteKnowledgeDbCommand request,
        CancellationToken cancellationToken
    )
    {
        var toDeletes = dbContext.KnowledgeDbs.Where(p => request.Ids.Contains(p.Id));

        foreach (var item in toDeletes)
        {
            item.AddDomainEvent(new KnowledgeDbDeletedEvent(item));
            dbContext.KnowledgeDbs.Remove(item); // todo: executedelete
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        
        // 知识库删除后，其下所有知识文档、向量全部删除
        var kbDocFileIds = await dbContext.KbDocFiles
            .Where(p => request.Ids.Contains(p.KbId))
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);
        if(kbDocFileIds.Count == 0) return Unit.Value;
        await mediator.Send(new DeleteKbDocFileCommand(kbDocFileIds));
        
        return Unit.Value;
    }
}
