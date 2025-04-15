using AutoMapper;
using Mediator;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.AIModels.Commands;
using YmKB.Application.Features.AIModels.EventHandlers;
using YmKB.Application.Features.KnowledgeDbs.EventHandlers;
using YmKB.Application.Pipeline;

namespace YmKB.Application.Features.KnowledgeDbs.Commands;

public record UpdateKnowledgeDbCommand(
    string Id,
    string? Icon,
    string Name,
    string Description,
    string? ChatModelID,
    string? EmbeddingModelID,
    int MaxTokensPerParagraph,
    int MaxTokensPerLine,
    int OverlappingTokens
) : IFusionCacheRefreshRequest<Unit>, IRequiresValidation
{
    public IEnumerable<string>? Tags => [ "KnowledgeDbs" ];
}

public class UpdateKnowledgeDbCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<UpdateKnowledgeDbCommand, Unit>
{
    public async ValueTask<Unit> Handle(
        UpdateKnowledgeDbCommand request,
        CancellationToken cancellationToken
    )
    {
        var toUpdate = await context.KnowledgeDbs.FindAsync([ request.Id ], cancellationToken);
        if (toUpdate == null)
        {
            throw new KeyNotFoundException($"AIModel with Id '{request.Id}' was not found.");
        }
        mapper.Map(request, toUpdate);
        toUpdate.AddDomainEvent(new KnowledgeDbUpdatedEvent(toUpdate));
        context.KnowledgeDbs.Update(toUpdate);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
