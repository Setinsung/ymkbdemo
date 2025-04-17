using AutoMapper;
using Mediator;
using Microsoft.EntityFrameworkCore;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.KbApps.EventHandlers;
using YmKB.Application.Pipeline;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.KbApps.Commands;

public record UpdateKbAppCommand(
    string Id,
    string Name,
    string Description,
    string? Icon,
    KbAppType KbAppType,
    string? ChatModelId,
    string? EmbeddingModelId,
    double Temperature,
    string? Prompt,
    string? ApiFunctionList,
    string? NativeFunctionList,
    string? KbIdList,
    double Relevance,
    int MaxAskPromptSize,
    int MaxMatchesCount,
    int AnswerTokens,
    string PromptTemplate,
    string? NoReplyFoundTemplate
) : IFusionCacheRefreshRequest<string>, IRequiresValidation
{
    public IEnumerable<string>? Tags => [ "KbApps" ];
}

public class UpdateKbAppCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<UpdateKbAppCommand, string>
{
    public async ValueTask<string> Handle(
        UpdateKbAppCommand request,
        CancellationToken cancellationToken
    )
    {
        var toUpdate = await context
            .KbApps
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (toUpdate == null)
        {
            throw new KeyNotFoundException($"KbApp with Id '{request.Id}' was not found.");
        }

        mapper.Map(request, toUpdate);
        toUpdate.AddDomainEvent(new KbAppUpdatedEvent(toUpdate));
        await context.SaveChangesAsync(cancellationToken);
        return toUpdate.Id;
    }
}
