using AutoMapper;
using Mediator;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.KnowledgeDbs.EventHandlers;
using YmKB.Application.Pipeline;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.KnowledgeDbs.Commands;

public record CreateKnowledgeDbCommand(
    string? Icon,
    string Name,
    string Description,
    string? ChatModelID,
    string? EmbeddingModelID,
    int MaxTokensPerParagraph,
    int MaxTokensPerLine,
    int OverlappingTokens
) : IFusionCacheRefreshRequest<string>, IRequiresValidation
{
    public IEnumerable<string>? Tags => [ "KnowledgeDbs" ];
}

public class CreateKnowledgeDbCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<CreateKnowledgeDbCommand, string>
{
    public async ValueTask<string> Handle(
        CreateKnowledgeDbCommand request,
        CancellationToken cancellationToken
    )
    {
        var toCreate = mapper.Map<KnowledgeDb>(request);
        toCreate.AddDomainEvent(new KnowledgeDbCreatedEvent(toCreate));
        context.KnowledgeDbs.Add(toCreate);
        await context.SaveChangesAsync(cancellationToken);
        return toCreate.Id;
    }
}
