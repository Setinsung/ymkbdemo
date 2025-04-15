// Summary:
// This file defines a command and its handler for updating AIModel details in the database.
// The UpdateAIModelCommand encapsulates the necessary data to update a AIModel, while the
// UpdateAIModelCommandHandler validates the AIModel's existence, updates its details,
// triggers a domain event such as AIModelUpdatedEvent, and commits the changes.

using Mediator;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.AIModels.EventHandlers;
using YmKB.Application.Pipeline;
using YmKB.Domain.Entities;
using YmKB.Domain.Entities.Trial;

namespace YmKB.Application.Features.AIModels.Commands;

// Command object that encapsulates the data required to update a AIModel.
// Each field corresponds to a property in AIModelDto.
public record UpdateAIModelCommand(
    string Id,
    AIModelType AIModelType,
    string Endpoint,
    string ModelName,
    string ModelKey,
    string ModelDescription
) : IFusionCacheRefreshRequest<Unit>, IRequiresValidation
{
    public IEnumerable<string>? Tags => [ "AIModels" ];
}

public class UpdateAIModelCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateAIModelCommand, Unit>
{
    public async ValueTask<Unit> Handle(
        UpdateAIModelCommand request,
        CancellationToken cancellationToken
    )
    {
        var toUpdate = await context.AIModels.FindAsync([ request.Id ], cancellationToken);
        if (toUpdate == null)
        {
            throw new KeyNotFoundException($"AIModel with Id '{request.Id}' was not found.");
        }

        toUpdate.AIModelType = request.AIModelType;
        toUpdate.Endpoint = request.Endpoint;
        toUpdate.ModelName = request.ModelName;
        toUpdate.ModelKey = request.ModelKey;
        toUpdate.ModelDescription = request.ModelDescription;
        toUpdate.AddDomainEvent(new AIModelUpdatedEvent(toUpdate));
        context.AIModels.Update(toUpdate);

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
