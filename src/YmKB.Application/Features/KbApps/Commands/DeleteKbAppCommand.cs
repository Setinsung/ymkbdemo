using AutoMapper;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.KbApps.EventHandlers;
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
    IApplicationDbContext dbContext
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

        return Unit.Value;
    }
}
