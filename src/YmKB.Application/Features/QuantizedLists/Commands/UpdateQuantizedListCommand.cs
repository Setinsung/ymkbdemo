using AutoMapper;
using Mediator;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.QuantizedLists.EventHandlers;
using YmKB.Application.Pipeline;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.QuantizedLists.Commands;

public record UpdateQuantizedListCommand(
    string Id,
    string KbId,
    string KbDocFileId,
    string Remark,
    QuantizedListState Status = QuantizedListState.Pending,
    DateTime? ProcessTime = null
) : IFusionCacheRefreshRequest<Unit>, IRequiresValidation
{
    public IEnumerable<string>? Tags => [ "QuantizedLists" ];
}

public class UpdateQuantizedListCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<UpdateQuantizedListCommand, Unit>
{
    public async ValueTask<Unit> Handle(
        UpdateQuantizedListCommand request,
        CancellationToken cancellationToken
    )
    {
        var toUpdate = await context.QuantizedLists.FindAsync([ request.Id ], cancellationToken);
        if (toUpdate == null)
        {
            throw new KeyNotFoundException($"AIModel with Id '{request.Id}' was not found.");
        }
        mapper.Map(request, toUpdate);
        toUpdate.AddDomainEvent(new QuantizedListUpdatedEvent(toUpdate));
        context.QuantizedLists.Update(toUpdate);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
