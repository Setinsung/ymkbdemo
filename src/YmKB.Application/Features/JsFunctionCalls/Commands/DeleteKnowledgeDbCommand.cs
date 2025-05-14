using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.KbDocFiles.Commands;
using YmKB.Application.Features.JsFunctionCalls.EventHandlers;
using YmKB.Application.Pipeline;

namespace YmKB.Application.Features.JsFunctionCalls.Commands;

public record DeleteJsFunctionCallCommand(params IEnumerable<string> Ids)
    : IFusionCacheRefreshRequest<Unit>,
        IRequiresValidation
{
    public IEnumerable<string>? Tags => [ "JsFunctionCalls" ];
}

public class DeleteJsFunctionCallCommandHandler(
    ILogger<DeleteJsFunctionCallCommandHandler> logger,
    IMediator mediator,
    IApplicationDbContext dbContext
) : IRequestHandler<DeleteJsFunctionCallCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteJsFunctionCallCommand request,
        CancellationToken cancellationToken
    )
    {
        var toDeletes = dbContext.JsFunctionCalls.Where(p => request.Ids.Contains(p.Id));

        foreach (var item in toDeletes)
        {
            item.AddDomainEvent(new JsFunctionCallDeletedEvent(item));
            dbContext.JsFunctionCalls.Remove(item); // todo: executedelete
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
