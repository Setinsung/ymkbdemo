using AutoMapper;
using Mediator;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.QuantizedLists.EventHandlers;
using YmKB.Application.Pipeline;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.QuantizedLists.Commands;

public record CreateQuantizedListCommand(
    string KbId,
    string KbDocFileId,
    string Remark
) : IFusionCacheRefreshRequest<string>, IRequiresValidation
{
    public IEnumerable<string>? Tags => [ "QuantizedLists" ];
}

public class CreateQuantizedListCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<CreateQuantizedListCommand, string>
{
    public async ValueTask<string> Handle(
        CreateQuantizedListCommand request,
        CancellationToken cancellationToken
    )
    {
        var toCreate = mapper.Map<QuantizedList>(request);
        toCreate.AddDomainEvent(new QuantizedListCreatedEvent(toCreate));
        context.QuantizedLists.Add(toCreate);
        await context.SaveChangesAsync(cancellationToken);
        return toCreate.Id;
    }
}
