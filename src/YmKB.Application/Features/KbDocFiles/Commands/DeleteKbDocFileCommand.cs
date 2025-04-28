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
    IApplicationDbContext dbContext
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

        return Unit.Value;
    }
}
