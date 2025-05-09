using AutoMapper;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.KernelMemory;
using YmKB.Application.Contracts;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.KbDocFiles.DTOs;
using YmKB.Application.Models;

namespace YmKB.Application.Features.KbDocFiles.Queries;

public record KbDocFileVectorsQuery(string KbDocFileId, int PageNumber, int PageSize)
    : IRequest<PaginatedResult<KbDocFileVectorDto>>;

public class KbDocFileVectorsQueryHandler(
    IApplicationDbContext dbContext,
    IAIKernelCreateService aiKernelCreateService,
    IMapper mapper
) : IRequestHandler<KbDocFileVectorsQuery, PaginatedResult<KbDocFileVectorDto>>
{
    public async ValueTask<PaginatedResult<KbDocFileVectorDto>> Handle(
        KbDocFileVectorsQuery request,
        CancellationToken cancellationToken
    )
    {
        // 获取到关联的知识库，模型配置
        var kbId = await dbContext
            .KbDocFiles
            .Where(e => e.Id == request.KbDocFileId)
            .Select(e => e.KbId)
            .FirstOrDefaultAsync();
        if (kbId == null)
            throw new KeyNotFoundException("文档不存在");
        var kb = await dbContext.KnowledgeDbs.FirstOrDefaultAsync(e => e.Id == kbId);
        if (kb == null)
            throw new KeyNotFoundException("知识库不存在");
        var chatModel = await dbContext.AIModels.FirstOrDefaultAsync(e => e.Id == kb.ChatModelID);
        var ebdModel = await dbContext
            .AIModels
            .FirstOrDefaultAsync(e => e.Id == kb.EmbeddingModelID);
        if (chatModel is null)
            throw new InvalidOperationException($"模型不存在：{kb.ChatModelID}");
        if (ebdModel is null)
            throw new InvalidOperationException($"模型不存在：{kb.EmbeddingModelID}");

        var memoryServerless = aiKernelCreateService.CreateMemoryServerless(chatModel, ebdModel);
        var memoryDbs = memoryServerless.Orchestrator.GetMemoryDbs();
        // dataCount 未获取
        List<KbDocFileVectorDto> kbDocFileVectorDtos =  [ ];
        foreach (var memoryDb in memoryDbs)
        {
            // 通过pageSize和page获取到最大数量
            var limit = request.PageSize * request.PageNumber;
            if (limit < 10)
                limit = 10;
            var filter = new MemoryFilter().ByDocument(request.KbDocFileId);
            var size = 0;
            await foreach (
                var item in memoryDb.GetListAsync(
                    "kb",
                    new List<MemoryFilter>() { filter },
                    limit,
                    true
                )
            )
            {
                size++;
                if (size < request.PageSize * (request.PageNumber - 1))
                    continue;
                if (size > request.PageSize * request.PageNumber)
                    break;
                kbDocFileVectorDtos.Add(
                    new KbDocFileVectorDto()
                    {
                        Id = item.Id,
                        Index = size,
                        Content = item.Payload["text"].ToString() ?? string.Empty,
                        KbDocFileId = item.Tags["KbDocFileId"].FirstOrDefault() ?? string.Empty,
                    }
                );
            }
        }
        return new PaginatedResult<KbDocFileVectorDto>(
            kbDocFileVectorDtos,
            kbDocFileVectorDtos.Count,
            request.PageNumber,
            request.PageSize
        );
    }
}
