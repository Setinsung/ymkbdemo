using AutoMapper;
using Mediator;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.AIModels.EventHandlers;
using YmKB.Application.Pipeline;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.AIModels.Commands;

public record CreateAIModelCommand(
    AIModelType AIModelType,
    string Endpoint,
    string ModelName,
    string ModelKey,
    string ModelDescription
) : IFusionCacheRefreshRequest<string>, IRequiresValidation
{
    public IEnumerable<string>? Tags => [ "AIModels" ];
}

public class CreateAIModelCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<CreateAIModelCommand, string>
{
    public async ValueTask<string> Handle(
        CreateAIModelCommand request,
        CancellationToken cancellationToken
    )
    {
        var toCreate = mapper.Map<AIModel>(request);
        toCreate.AddDomainEvent(new AIModelCreatedEvent(toCreate));
        context.AIModels.Add(toCreate);
        await context.SaveChangesAsync(cancellationToken);
        return toCreate.Id;
    }
}
