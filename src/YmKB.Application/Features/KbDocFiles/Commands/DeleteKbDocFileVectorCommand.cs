using Mediator;
using Microsoft.Extensions.Logging;
using YmKB.Application.Contracts;
using YmKB.Application.Pipeline;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.KbDocFiles.Commands;

/// <summary>
/// 根据向量数据库中的DocumentId删除向量
/// </summary>
/// <param name="DocumentId"></param>
public record DeleteKbDocFileVectorCommand(string DocumentId)
    : IRequest<Unit>,
        IRequiresValidation;

public class DeleteKbDocFileVectorCommandHandler(
    ILogger<DeleteKbDocFileVectorCommandHandler> logger,
    IAIKernelCreateService aiKernelCreateService
) : IRequestHandler<DeleteKbDocFileVectorCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteKbDocFileVectorCommand request,
        CancellationToken cancellationToken
    )
    {
        var memoryServerless = aiKernelCreateService.CreateMemoryServerless(
            new AIModel(),
            new AIModel()
        );
        await memoryServerless.DeleteDocumentAsync(request.DocumentId, "kb");
        return Unit.Value;
    }
}
