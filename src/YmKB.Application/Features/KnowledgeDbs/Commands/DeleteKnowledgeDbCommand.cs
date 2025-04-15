using Mediator;
using Microsoft.Extensions.Logging;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
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
            dbContext.KnowledgeDbs.Remove(item); // todo: executedeelte
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
