using AutoMapper;
using Mediator;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.JsFunctionCalls.EventHandlers;
using YmKB.Application.Pipeline;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.JsFunctionCalls.Commands;

public record CreateJsFunctionCallCommand(
    string Name,
    string Description,
    string ScriptContent,
    string MainFuncName,
    List<JsFunctionParameter> Parameters
) : IFusionCacheRefreshRequest<string>, IRequiresValidation
{
    public IEnumerable<string>? Tags => [ "JsFunctionCalls" ];
}

public class CreateJsFunctionCallCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<CreateJsFunctionCallCommand, string>
{
    public async ValueTask<string> Handle(
        CreateJsFunctionCallCommand request,
        CancellationToken cancellationToken
    )
    {
        var toCreate = mapper.Map<JsFunctionCall>(request);
        toCreate.AddDomainEvent(new JsFunctionCallCreatedEvent(toCreate));
        context.JsFunctionCalls.Add(toCreate);
        await context.SaveChangesAsync(cancellationToken);
        return toCreate.Id;
    }
}
