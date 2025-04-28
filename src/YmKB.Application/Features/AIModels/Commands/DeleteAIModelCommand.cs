using Mediator;
using Microsoft.Extensions.Logging;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.AIModels.EventHandlers;
using YmKB.Application.Pipeline;

namespace YmKB.Application.Features.AIModels.Commands;

public record DeleteAIModelCommand(params IEnumerable<string> Ids)
    : IFusionCacheRefreshRequest<Unit>,
        IRequiresValidation
{
    public IEnumerable<string>? Tags => [ "AIModels" ];
}

public class DeleteAIModelCommandHandler(
    ILogger<DeleteAIModelCommandHandler> logger,
    IApplicationDbContext dbContext
) : IRequestHandler<DeleteAIModelCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteAIModelCommand request,
        CancellationToken cancellationToken
    )
    {
        var toDeletes = dbContext.AIModels.Where(p => request.Ids.Contains(p.Id));

        foreach (var item in toDeletes)
        {
            item.AddDomainEvent(new AIModelDeletedEvent(item));
            dbContext.AIModels.Remove(item); // todo: executedelete
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
