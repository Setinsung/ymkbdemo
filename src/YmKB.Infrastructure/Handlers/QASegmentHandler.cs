using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory.AI;
using Microsoft.KernelMemory.Configuration;
using Microsoft.KernelMemory.Diagnostics;
using Microsoft.KernelMemory.Pipeline;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Text;
using YmKB.Domain.Entities;
using YmKB.Infrastructure.Services;
namespace YmKB.Infrastructure.Handlers;

public class QASegmentHandler : IPipelineStepHandler
{
    public string StepName { get; }

    public static readonly AsyncLocal<AIModel> Context = new();

    private string _QAPromptTemplate = """"
                                      
                                      我会给你一段文本，学习它们，并整理学习成果，要求为：
                                      1. 提出最多 20 个问题。
                                      2. 给出每个问题的答案。
                                      3. 答案要详细完整，答案可以包含普通文字、链接、代码、表格、公示、媒体链接等 markdown 元素。
                                      4. 按格式返回多个问题和答案:
                                      
                                      Q1: 问题。
                                      A1: 答案。
                                      Q2:
                                      A2:
                                      ……
                                      
                                      我的文本："""{{$input}}"""
                                      """";
    
    private readonly ILogger<QASegmentHandler> _log;
    private readonly TextPartitioningOptions _options;
    private readonly IPipelineOrchestrator _orchestrator;
#pragma warning disable KMEXP00
    private readonly ITextTokenizer _tokenCounter;
#pragma warning restore KMEXP00
    private readonly AIKernelCreateService _aiKernelCreateService;
    
    public QASegmentHandler(
        string stepName,
        IPipelineOrchestrator orchestrator, AIKernelCreateService aiKernelCreateService,
        TextPartitioningOptions? options = null,
        ILogger<QASegmentHandler>? log = null
    )
    {
        StepName = stepName;
        _orchestrator = orchestrator;
        _aiKernelCreateService = aiKernelCreateService;
        _options = options ?? new TextPartitioningOptions();
        _options.Validate();

        _log = log ?? DefaultLogger<QASegmentHandler>.Instance;
        _tokenCounter = new GPT4Tokenizer();
    }
    
    public async Task<(ReturnType returnType, DataPipeline updatedPipeline)> InvokeAsync(DataPipeline pipeline, CancellationToken cancellationToken = default)
{
        _log.LogDebug("对文本进行分区、管道 '{0}/{1}'", pipeline.Index, pipeline.DocumentId);

        if (pipeline.Files.Count == 0)
        {
            _log.LogWarning("管道 '{0}/{1}': 没有要处理的文件，进入下一个管道步骤。",
                pipeline.Index, pipeline.DocumentId);
            return (ReturnType.Success, pipeline);
        }

        foreach (var uploadedFile in pipeline.Files)
        {
            // 跟踪生成的新文件（循环时无法编辑 originalFile.GeneratedFiles）
            Dictionary<string, DataPipeline.GeneratedFileDetails> newFiles = new();

            foreach (var generatedFile in uploadedFile
                         .GeneratedFiles)
            {
                var file = generatedFile.Value;
                if (file.AlreadyProcessedBy(this))
                {
                    _log.LogTrace("文件 {0} 已由此处理程序处理", file.Name);
                    continue;
                }

                // 仅对原始文本进行分区
                if (file.ArtifactType != DataPipeline.ArtifactTypes.ExtractedText)
                {
                    _log.LogTrace("正在跳过文件 {0} (非原始文本)", file.Name);
                    continue;
                }

                // 根据文件类型使用不同的分区策略
                List<string> partitions = new();
                List<string> sentences = new();
                var partitionContent = await _orchestrator
                    .ReadFileAsync(pipeline, file.Name, cancellationToken).ConfigureAwait(false);

                // 跳过空分区。此外：如果没有字节，partitionContent.ToString（） 会引发异常。
                if (partitionContent.ToArray().Length == 0) continue;

                switch (file.MimeType)
                {
                    case MimeTypes.PlainText:
                    case MimeTypes.MarkDown:
                    {
                        _log.LogDebug("对文本文件进行分区 {0}", file.Name);
                        var content = partitionContent.ToString();

                        var chatModel = Context.Value;

                        await foreach (var item in QaAsync(_QAPromptTemplate, content,chatModel, _aiKernelCreateService )
                                           .WithCancellation(cancellationToken))
                        {
                            partitions.Add(item);
                            sentences.Add(item);
                        }

                        break;
                    }
                    default:
                        _log.LogWarning("文件 {0} 无法分区, 类型 '{1}' 不支持", file.Name,
                            file.MimeType);
                        // 不对其他文件进行分区
                        continue;
                }

                if (partitions.Count == 0) continue;

                _log.LogDebug("保存 {0} 文件分区", partitions.Count);
                for (var partitionNumber = 0; partitionNumber < partitions.Count; partitionNumber++)
                {
                    // TODO: 将分区转换为具有更多详细信息的对象，例如页码
                    var text = partitions[partitionNumber];
                    var sectionNumber = 0; // TODO: 使用它来存储页码（如果有）
                    BinaryData textData = new(text);

                    var tokenCount = _tokenCounter.CountTokens(text);
                    _log.LogDebug("分区大小: {0} 令牌", tokenCount);

                    var destFile = uploadedFile.GetPartitionFileName(partitionNumber);
                    await _orchestrator.WriteFileAsync(pipeline, destFile, textData, cancellationToken)
                        .ConfigureAwait(false);

                    var destFileDetails = new DataPipeline.GeneratedFileDetails
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        ParentId = uploadedFile.Id,
                        Name = destFile,
                        Size = text.Length,
                        MimeType = MimeTypes.PlainText,
                        ArtifactType = DataPipeline.ArtifactTypes.TextPartition,
                        PartitionNumber = partitionNumber,
                        SectionNumber = sectionNumber,
                        Tags = pipeline.Tags,
                        ContentSHA256 = CalculateSHA256(textData)
                    };
                    newFiles.Add(destFile, destFileDetails);
                    destFileDetails.MarkProcessedBy(this);
                }

                file.MarkProcessedBy(this);
            }

            // 将新文件添加到管道状态
            foreach (var file in newFiles) uploadedFile.GeneratedFiles.Add(file.Key, file.Value);
        }

        return (ReturnType.Success, pipeline);
    }

    /// <summary>
    /// 通过大模型ai将文本切分成段落，每个段落不超过 4000 个字符。
    /// 每个段落作为输入传递给 QA 函数，该函数使用 AI 模型生成答案，最后答案组异步返回
    /// </summary>
    /// <param name="prompt"></param>
    /// <param name="value"></param>
    /// <param name="chatModel"></param>
    /// <param name="aiKernelCreateService"></param>
    /// <returns></returns>
    public static async IAsyncEnumerable<string> QaAsync(
        string prompt,
        string value,
        AIModel chatModel,
        AIKernelCreateService aiKernelCreateService
    )
    {
        var kernel = aiKernelCreateService.CreateFunctionKernel(chatModel);
        var qaFunction = kernel.CreateFunctionFromPrompt(
            prompt,
            functionName: "QA",
            description: "QA问答"
        );

#pragma warning disable SKEXP0050
        var lines = TextChunker.SplitPlainTextLines(value, 299);
        var paragraphs = TextChunker.SplitPlainTextParagraphs(lines, 4000);
#pragma warning restore SKEXP0050

        foreach (var paragraph in paragraphs)
        {
            var result = await kernel.InvokeAsync(
                qaFunction,
                new KernelArguments { { "input", paragraph } }
            );

            yield return result.GetValue<string>();
        }
    }
    
    public static string CalculateSHA256(BinaryData data)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(data.ToArray());
        return Convert.ToHexStringLower(hashBytes);
    }
}