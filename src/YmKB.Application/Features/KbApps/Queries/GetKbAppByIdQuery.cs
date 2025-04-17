using AutoMapper;
using Mediator;
using Microsoft.EntityFrameworkCore;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.AIModels.Queries;
using YmKB.Application.Features.KbApps.DTOs;

namespace YmKB.Application.Features.KbApps.Queries;

public record GetKbAppByIdQuery(string Id) : IFusionCacheRequest<KbAppDto?>
{
    public string CacheKey => $"KbApp_{Id}";

    public IEnumerable<string>? Tags => [ "KbApps" ];
}

public class GetKbAppByIdQueryHandler(
    IApplicationDbContext dbContext,
    IMapper mapper,
    IMediator mediator
) : IRequestHandler<GetKbAppByIdQuery, KbAppDto?>
{
    public async ValueTask<KbAppDto?> Handle(
        GetKbAppByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var find = await dbContext
            .KbApps
            .Where(p => p.Id == request.Id)
            .Select(
                p =>
                    new KbAppDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Icon = p.Icon,
                        KbAppType = p.KbAppType,
                        ChatModelId = p.ChatModelId,
                        EmbeddingModelId = p.EmbeddingModelId,
                        Temperature = p.Temperature,
                        Prompt = p.Prompt,
                        ApiFunctionList = p.ApiFunctionList,
                        NativeFunctionList = p.NativeFunctionList,
                        KbIdList = p.KbIdList,
                        Relevance = p.Relevance,
                        MaxAskPromptSize = p.MaxAskPromptSize,
                        MaxMatchesCount = p.MaxMatchesCount,
                        AnswerTokens = p.AnswerTokens,
                        PromptTemplate = p.PromptTemplate,
                        NoReplyFoundTemplate = p.NoReplyFoundTemplate
                    }
            )
            .SingleOrDefaultAsync(cancellationToken);

        if (find == null)
        {
            throw new KeyNotFoundException($"KbApp with Id '{request.Id}' was not found."); // Handle not found case
        }

        if (!string.IsNullOrEmpty(find.ChatModelId))
        {
            find.ChatModel = await mediator.Send(
                new GetAIModelByIdQuery(find.ChatModelId),
                cancellationToken
            );
        }
        if (!string.IsNullOrEmpty(find.EmbeddingModelId))
        {
            find.EmbeddingModel = await mediator.Send(
                new GetAIModelByIdQuery(find.EmbeddingModelId),
                cancellationToken
            );
        }
        var data = mapper.Map<KbAppDto>(find);

        return data;
    }
}
