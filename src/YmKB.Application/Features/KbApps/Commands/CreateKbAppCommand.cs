using AutoMapper;
using Mediator;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.KbApps.EventHandlers;
using YmKB.Application.Pipeline;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.KbApps.Commands;

public record CreateKbAppCommand(
    string Name,
    string Description,
    string? Icon,
    KbAppType KbAppType,
    string? ChatModelId,
    string? EmbeddingModelId,
    string? Prompt,
    string? ApiFunctionList,
    string? NativeFunctionList,
    string? KbIdList,
    string PromptTemplate,
    string? NoReplyFoundTemplate,
    double Temperature = 0.5,
    double Relevance = 0.4,
    int MaxAskPromptSize = 2048,
    int MaxMatchesCount = 10,
    int AnswerTokens = 2048
) : IFusionCacheRefreshRequest<string>, IRequiresValidation
{
    public IEnumerable<string>? Tags => [ "KbApps" ];
}

public class CreateKbAppCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<CreateKbAppCommand, string>
{
    public async ValueTask<string> Handle(
        CreateKbAppCommand request,
        CancellationToken cancellationToken
    )
    {
        var toCreate = mapper.Map<KbApp>(request);
        toCreate.AddDomainEvent(new KbAppCreatedEvent(toCreate));
        context.KbApps.Add(toCreate);
        await context.SaveChangesAsync(cancellationToken);
        return toCreate.Id;
    }
}
