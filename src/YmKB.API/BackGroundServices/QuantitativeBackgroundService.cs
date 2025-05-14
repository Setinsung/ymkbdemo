using System.Threading.Channels;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Handlers;
using YmKB.Application.Contracts;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.KnowledgeDbs.Queries;
using YmKB.Application.Features.QuantizedLists.Commands;
using YmKB.Domain.Entities;
using YmKB.Infrastructure.Handlers;
using YmKB.Infrastructure.Services;

namespace YmKB.API.BackGroundServices;

public class QuantitativeBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<QuantitativeBackgroundService> _logger;

    /// <summary>
    /// 量化任务数量
    /// </summary>
    private static int _maxTask = 1;
    
    /// <summary>
    /// 当前任务数量
    /// </summary>
    private static int _currentTask;


    /// <summary>
    /// 文档列
    /// </summary>
    private static readonly Channel<string> KbDocFileChannel = Channel.CreateBounded<string>(
        new BoundedChannelOptions(1000) { SingleReader = true, SingleWriter = false }
    );

    public QuantitativeBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<QuantitativeBackgroundService> logger
    )
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var quantizeMaxTaskCount = Environment.GetEnvironmentVariable("QUANTIZE_MAX_TASK");
        if (!string.IsNullOrEmpty(quantizeMaxTaskCount))
            int.TryParse(quantizeMaxTaskCount, out _maxTask);
        if (_maxTask < 0)
            _maxTask = 1;

        // 首次启动
        await LoadingKbDocFileAsync();
        
        
        var tasks = new List<Task>();
        for (var i = 0; i < _maxTask; i++) tasks.Add(Task.Factory.StartNew(KbDocFileStartHandlerAsync, stoppingToken));

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// 首次启动时，加载量化失败且存在的文档
    /// </summary>
    private async Task LoadingKbDocFileAsync()
    {
        using var asyncServiceScope = _serviceProvider.CreateScope();
        var dbContext = asyncServiceScope
            .ServiceProvider
            .GetRequiredService<IApplicationDbContext>();

        var failedkbDocFileIds = await dbContext
            .KbDocFiles
            .Where(
                e =>
                    dbContext
                        .QuantizedLists
                        .Where(
                            r =>
                                r.Status == QuantizedListState.Fail
                                || r.Status == QuantizedListState.Pending
                        )
                        .Select(r => r.KbDocFileId)
                        .Contains(e.Id)
            )
            .Select(e => e.Id)
            .ToListAsync();
        foreach (var kbDocFileId in failedkbDocFileIds)
        {
            await AddKbDocFileAsync(kbDocFileId);
        }
    }
    
    /// <summary>
    /// 向量化队列中添加文档
    /// </summary>
    /// <param name="kbDocFileId"></param>
    public static async Task AddKbDocFileAsync(string kbDocFileId)
    {
        await KbDocFileChannel.Writer.WriteAsync(kbDocFileId);
    }
    
    /// <summary>
    /// 循环消费队列
    /// </summary>
    private async Task KbDocFileStartHandlerAsync()
    {
        using var asyncServiceScope = _serviceProvider.CreateScope();
        while (await KbDocFileChannel.Reader.WaitToReadAsync())
        {
            Interlocked.Increment(ref _currentTask);
            _logger.LogInformation($"当前量化任务数量：{_currentTask}");
            var kbDocFileId = await KbDocFileChannel.Reader.ReadAsync();
            await HandlerAsync(kbDocFileId, asyncServiceScope.ServiceProvider);
            Interlocked.Decrement(ref _currentTask);
        }
    }
    
    /// <summary>
    /// 处理量化核心逻辑
    /// </summary>
    /// <param name="kbDocFileId"></param>
    /// <param name="service"></param>
    private async ValueTask HandlerAsync(string kbDocFileId, IServiceProvider service)
    {
        var dbContext = service.GetRequiredService<IApplicationDbContext>();
        var aiKernelCreateService = service.GetRequiredService<IAIKernelCreateService>();
        var mediator = service.GetRequiredService<IMediator>();

        // 获取知识文档，关联的知识库，知识库配置的模型
        var kbDocFile = await dbContext.KbDocFiles.FirstOrDefaultAsync(e => e.Id == kbDocFileId);
        if(kbDocFile is null)
            throw new InvalidOperationException($"文档不存在：{kbDocFileId}");
        var kb = await dbContext.KnowledgeDbs.SingleOrDefaultAsync(e => e.Id == kbDocFile.KbId);
        if(kb is null) 
            throw new InvalidOperationException($"知识库不存在：{kbDocFile.KbId}");
        var chatModel = await dbContext.AIModels.FirstOrDefaultAsync(e => e.Id == kb.ChatModelID);
        var ebdModel = await dbContext.AIModels.FirstOrDefaultAsync(e => e.Id == kb.EmbeddingModelID);
        if (chatModel is null)
            throw new InvalidOperationException($"模型不存在：{kb.ChatModelID}");
        if (ebdModel is null)
            throw new InvalidOperationException($"模型不存在：{kb.EmbeddingModelID}");
        
        // 创建量化队列，如果存在则更新状态，否则创建
        string qlId;
        string remark = $"创建量化任务：{kbDocFile.FileName} {kbDocFile.Url} {kbDocFile.Id}";
        var existQuantizedItem = await dbContext.QuantizedLists.FirstOrDefaultAsync(x => x.KbId == kb.Id && x.KbDocFileId == kbDocFile.Id);
        if (existQuantizedItem is not null)
        {
            await mediator.Send(new UpdateQuantizedListCommand(existQuantizedItem.Id, kb.Id, kbDocFile.Id, remark));
            // existQuantizedItem.Status = QuantizedListState.Pending;
            // existQuantizedItem.Remark = remark;
            // existQuantizedItem.ProcessTime = null;
            // await dbContext.SaveChangesAsync();
            qlId = existQuantizedItem.Id;
        }
        else
        {
            var newQuantizedItem = new QuantizedList
            {
                KbId = kb.Id,
                KbDocFileId = kbDocFile.Id,
                Remark = remark,
                Status = QuantizedListState.Pending
            };

            qlId = await mediator.Send(new CreateQuantizedListCommand(kb.Id, kbDocFile.Id, remark));
            // await dbContext.QuantizedLists.AddAsync(newQuantizedItem);
            // await dbContext.SaveChangesAsync();
            // qlId = newQuantizedItem.Id;
        }
        
        
        // 根据模型创建内存服务，然后开始量化
        try
        {
            var serverless = aiKernelCreateService.CreateMemoryServerless(new SearchClientConfig(),
                kb.MaxTokensPerParagraph, kb.OverlappingTokens,
                chatModel,
                ebdModel);
            _logger.LogInformation($"开始量化：{kbDocFile.FileName} {kbDocFile.Url} {kbDocFile.Id}");
            List<string> step = [];

            // QA切分
            if (kbDocFile.SegmentPattern == SegmentPattern.QA)
            {
                QASegmentHandler.Context.Value = chatModel;
                var stepName = kbDocFile.Id;
                serverless.Orchestrator.AddHandler<TextExtractionHandler>("extract_text");
                serverless.Orchestrator.AddHandler<QASegmentHandler>(stepName);
                serverless.Orchestrator.AddHandler<GenerateEmbeddingsHandler>("generate_embeddings");
                serverless.Orchestrator.AddHandler<SaveRecordsHandler>("save_memory_records");
                step.Add("extract_text");
                step.Add(stepName);
                step.Add("generate_embeddings");
                step.Add("save_memory_records");
            }

            var result = string.Empty;
            if (kbDocFile.Type == "web")
            {
                result = await serverless.ImportWebPageAsync(kbDocFile.Url,
                    kbDocFile.Id,
                    new TagCollection
                    {
                        {
                            "kbId", kbDocFile.KbId
                        },
                        {
                            "KbDocFileId", kbDocFile.Id
                        }
                    }, "kb", step.ToArray());
            }
            else
            {
                var baseDirectory = Path.GetFullPath(Directory.GetCurrentDirectory());
                var filePath = Path.GetFullPath(Path.Combine(baseDirectory, kbDocFile.Url));
                result = await serverless.ImportDocumentAsync(filePath,
                    kbDocFile.Id,
                    new TagCollection
                    {
                        {
                            "kbId", kbDocFile.KbId
                        },
                        {
                            "KbDocFileId", kbDocFile.Id
                        }
                    }, "kb", step.ToArray());
            }

            await UpdateKbDocFileStatus(dbContext, kbDocFile.Id, QuantizationState.Accomplish);
            
            await UpdateQuantizedListStatus(dbContext, qlId,
                $"量化成功：{kbDocFile.FileName} {kbDocFile.Url} {kbDocFile.Id} {result}",
                QuantizedListState.Success);
        }
        catch (Exception e)
        {
            await UpdateQuantizedListStatus(dbContext,qlId,
                $"量化失败：{kbDocFile.FileName} {e.Message}",
                QuantizedListState.Fail);

            if (kbDocFile.Status != QuantizationState.Fail)
                await UpdateKbDocFileStatus(dbContext, kbDocFile.Id, QuantizationState.Fail);
        }
    }

    private Task UpdateKbDocFileStatus(IApplicationDbContext dbContext,string id, QuantizationState status)
    {
        return dbContext.KbDocFiles.Where(e => e.Id == id).ExecuteUpdateAsync(e => e
            .SetProperty(x => x.Status, status)
        );
    }

    private Task UpdateQuantizedListStatus(IApplicationDbContext dbContext, string id, string remark,  QuantizedListState status)
    {
        return dbContext.QuantizedLists.Where(e => e.Id == id).ExecuteUpdateAsync(e => e
           .SetProperty(x => x.Status, status)
           .SetProperty(x => x.Remark, remark)
           .SetProperty(x => x.ProcessTime, DateTime.UtcNow)
        );  
    }

}
