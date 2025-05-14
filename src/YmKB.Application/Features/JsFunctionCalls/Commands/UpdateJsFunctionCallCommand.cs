using AutoMapper;
using Mediator;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.AIModels.Commands;
using YmKB.Application.Features.AIModels.EventHandlers;
using YmKB.Application.Features.JsFunctionCalls.EventHandlers;
using YmKB.Application.Pipeline;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.JsFunctionCalls.Commands;

public record UpdateJsFunctionCallCommand(
    string Id,
    string Name,
    string Description,
    string ScriptContent,
    string MainFuncName,
    List<JsFunctionParameter> Parameters
) : IFusionCacheRefreshRequest<Unit>, IRequiresValidation
{
    public IEnumerable<string>? Tags => [ "JsFunctionCalls" ];
}

public class UpdateJsFunctionCallCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<UpdateJsFunctionCallCommand, Unit>
{
    public async ValueTask<Unit> Handle(
        UpdateJsFunctionCallCommand request,
        CancellationToken cancellationToken
    )
    {
        var toUpdate = await context.JsFunctionCalls.FindAsync([ request.Id ], cancellationToken);
        if (toUpdate == null)
        {
            throw new KeyNotFoundException($"AIModel with Id '{request.Id}' was not found.");
        }
        mapper.Map(request, toUpdate);
        toUpdate.AddDomainEvent(new JsFunctionCallUpdatedEvent(toUpdate));
        context.JsFunctionCalls.Update(toUpdate);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
