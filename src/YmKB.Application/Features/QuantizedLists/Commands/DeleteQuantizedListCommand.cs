using Mediator;
using Microsoft.Extensions.Logging;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.QuantizedLists.EventHandlers;
using YmKB.Application.Pipeline;

namespace YmKB.Application.Features.QuantizedLists.Commands;

public record DeleteQuantizedListCommand(params IEnumerable<string> Ids)
    : IFusionCacheRefreshRequest<Unit>,
        IRequiresValidation
{
    public IEnumerable<string>? Tags => [ "QuantizedLists" ];
}

public class DeleteQuantizedListCommandHandler(
    ILogger<DeleteQuantizedListCommandHandler> logger,
    IApplicationDbContext dbContext
) : IRequestHandler<DeleteQuantizedListCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteQuantizedListCommand request,
        CancellationToken cancellationToken
    )
    {
        var toDeletes = dbContext.QuantizedLists.Where(p => request.Ids.Contains(p.Id));

        foreach (var item in toDeletes)
        {
            item.AddDomainEvent(new QuantizedListDeletedEvent(item));
            dbContext.QuantizedLists.Remove(item); // todo: executedelete
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
