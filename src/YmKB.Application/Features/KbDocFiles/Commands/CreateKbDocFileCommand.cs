﻿using AutoMapper;
using Mediator;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.KbDocFiles.EventHandlers;
using YmKB.Application.Pipeline;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.KbDocFiles.Commands;

public record CreateKbDocFileCommand(
    string KbId,
    string FileName,
    string Url,
    long Size = 0,
    SegmentPattern? SegmentPattern = SegmentPattern.Subsection,
    string? Type = "file"
) : IFusionCacheRefreshRequest<string>, IRequiresValidation
{
    public IEnumerable<string>? Tags => [ "KbDocFiles" ];
}

public class CreateKbDocFileCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<CreateKbDocFileCommand, string>
{
    public async ValueTask<string> Handle(
        CreateKbDocFileCommand request,
        CancellationToken cancellationToken
    )
    {
        // 文件
        if (request.Type != "web")
        {
            var toCreate = mapper.Map<KbDocFile>(request);
            toCreate.DataCount = 0;
            toCreate.AddDomainEvent(new KbDocFileCreatedEvent(toCreate));
            context.KbDocFiles.Add(toCreate);
            await context.SaveChangesAsync(cancellationToken);
            return toCreate.Id;
        }
        // 网页链接
        else
        {
            var toCreate = mapper.Map<KbDocFile>(request);
            toCreate.DataCount = 0;
            toCreate.AddDomainEvent(new KbDocFileCreatedEvent(toCreate));
            context.KbDocFiles.Add(toCreate);
            await context.SaveChangesAsync(cancellationToken);
            return toCreate.Id;
        }

    }
}
