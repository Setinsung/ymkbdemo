using AutoMapper;
using Mediator;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.KbDocFiles.EventHandlers;
using YmKB.Application.Pipeline;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.KbDocFiles.Commands;

public record UpdateKbDocFileCommand(
    string Id,
    string KbId,
    string FileName,
    string Url,
    string Type,
    string Size,
    string DataCount,
    QuantizationState Status,
    SegmentPattern SegmentPattern
) : IFusionCacheRefreshRequest<Unit>, IRequiresValidation
{
    public IEnumerable<string>? Tags => [ "KbDocFiles" ];
}

public class UpdateKbDocFileCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<UpdateKbDocFileCommand, Unit>
{
    public async ValueTask<Unit> Handle(
        UpdateKbDocFileCommand request,
        CancellationToken cancellationToken
    )
    {
        var toUpdate = await context.KbDocFiles.FindAsync([ request.Id ], cancellationToken);
        if (toUpdate == null)
        {
            throw new KeyNotFoundException($"AIModel with Id '{request.Id}' was not found.");
        }
        mapper.Map(request, toUpdate);
        toUpdate.AddDomainEvent(new KbDocFileUpdatedEvent(toUpdate));
        context.KbDocFiles.Update(toUpdate);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
