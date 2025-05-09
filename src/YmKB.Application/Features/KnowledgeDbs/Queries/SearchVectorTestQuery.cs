using System.Diagnostics;
using AutoMapper;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.KernelMemory;
using YmKB.Application.Contracts;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.KnowledgeDbs.DTOs;

namespace YmKB.Application.Features.KnowledgeDbs.Queries;

public record SearchVectorTestQuery(string KbId, string Keywords, double MinRelevance = 0.0)
    : IRequest<SearchedVectorsDto>;

public class SearchVectorTestQueryHandler(
    IApplicationDbContext dbContext,
    IAIKernelCreateService aiKernelCreateService,
    IMapper mapper
) : IRequestHandler<SearchVectorTestQuery, SearchedVectorsDto>
{
    public async ValueTask<SearchedVectorsDto> Handle(
        SearchVectorTestQuery request,
        CancellationToken cancellationToken
    )
    {
        // 启动计时器，用于统计搜索耗时
        var stopwatch = Stopwatch.StartNew();

        // 获取请求的知识库和模型配置
        var kb = await dbContext.KnowledgeDbs.FirstOrDefaultAsync(e => e.Id == request.KbId);
        if (kb == null)
            throw new KeyNotFoundException("KnowledgeBase not found");
        var chatModel = await dbContext.AIModels.FirstOrDefaultAsync(e => e.Id == kb.ChatModelID);
        var ebdModel = await dbContext
            .AIModels
            .FirstOrDefaultAsync(e => e.Id == kb.EmbeddingModelID);
        if (chatModel is null)
            throw new InvalidOperationException($"模型不存在：{kb.ChatModelID}");
        if (ebdModel is null)
            throw new InvalidOperationException($"模型不存在：{kb.EmbeddingModelID}");

        // 创建向量内存搜索服务实例
        var memoryServerless = aiKernelCreateService.CreateMemoryServerless(chatModel, ebdModel);

        // 执行向量搜索
        var memorySearchRes = await memoryServerless.SearchAsync(
            request.Keywords,
            "kb",
            new MemoryFilter().ByTag("kbId", request.KbId),
            minRelevance: request.MinRelevance,
            limit: 5
        );
        stopwatch.Stop();

        SearchedVectorsDto result = new() { ElapsedTime = stopwatch.ElapsedMilliseconds };
        foreach (var item in memorySearchRes.Results)
        {
            // 每一个搜索结果可能包含多个分区（partitions），每个分区包含一个文本片段和相关度，集成到最终的搜索结果中
            result
                .Results
                .AddRange(
                    item.Partitions.Select(
                        e =>
                            new SearchVectorItem()
                            {
                                Content = e.Text,
                                DocumentId = item.DocumentId,
                                Relevance = e.Relevance,
                                KbDocFileId = e.Tags["kbDocFileId"].FirstOrDefault() ?? string.Empty
                            }
                    )
                );
        }
        
        // 补充文件路径和文件名信息
        var fileIds = result.Results.Select(e => e.KbDocFileId).ToList();

        var kbDocFiles = await dbContext
            .KbDocFiles
            .Where(e => fileIds.Contains(e.Id))
            .ToListAsync();
        foreach (var searchVectorItem in result.Results)
        {
            var file = kbDocFiles.FirstOrDefault(e => e.Id == searchVectorItem.KbDocFileId);
            searchVectorItem.Path = file?.Url;
            searchVectorItem.FileName = file?.FileName;
        }
        return result;
    }
}
