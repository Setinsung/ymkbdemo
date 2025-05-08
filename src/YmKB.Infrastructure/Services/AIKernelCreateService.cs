using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Configuration;
using Microsoft.KernelMemory.DocumentStorage.DevTools;
using Microsoft.SemanticKernel;
using YmKB.Domain.Entities;
using YmKB.Infrastructure.Configurations;
using YmKB.Infrastructure.Handlers;
using YmKB.JSFunctionCall;

namespace YmKB.Infrastructure.Services;

/// <summary>
/// AI内核创建服务
/// </summary>
public class AIKernelCreateService
{
    private static readonly JsFunctionCallContext JsFunctionCallContext = new();
    private readonly QdrantSettings _qdrantSettings;
    private readonly IOptions<QdrantSettings> _optionsQdrantSettings;

    // private static readonly OpenAICustomHttpClientHandler HttpClientHandler = new();

    public AIKernelCreateService(IOptions<QdrantSettings> qdrantSettings)
    {
        _optionsQdrantSettings = qdrantSettings;
        _qdrantSettings = qdrantSettings.Value;
    }

    public MemoryServerless CreateMemoryServerless(
        SearchClientConfig searchClientConfig,
        int maxTokensPerParagraph,
        int overlappingTokens,
        AIModel chatModel,
        AIModel embeddingModel
    )
    {
        var memoryServerless = new KernelMemoryBuilder()
            .WithQdrantMemoryDb(new QdrantConfig() { Endpoint = _qdrantSettings.Endpoint })
            .WithSearchClientConfig(searchClientConfig)
            .WithCustomTextPartitioningOptions(
                new TextPartitioningOptions
                {
                    MaxTokensPerParagraph = maxTokensPerParagraph,
                    OverlappingTokens = overlappingTokens
                }
            )
            .WithOpenAITextGeneration(
                new OpenAIConfig() { APIKey = chatModel.ModelKey, TextModel = chatModel.ModelName },
                null,
                new HttpClient(new OpenAICustomHttpClientHandler(chatModel.Endpoint))
            )
            .WithOpenAITextEmbeddingGeneration(
                new OpenAIConfig
                {
                    APIKey = embeddingModel.ModelKey,
                    EmbeddingModel = embeddingModel.ModelName
                },
                null,
                false,
                new HttpClient(new OpenAICustomHttpClientHandler(embeddingModel.Endpoint))
            )
            .WithSimpleFileStorage(SimpleFileStorageConfig.Persistent)
            .AddSingleton(new AIKernelCreateService(_optionsQdrantSettings))
            .Build<MemoryServerless>();
        return memoryServerless;
    }

    /// <summary>
    /// 仅用于操作的内存服务，不用于向量搜索
    /// </summary>
    /// <returns></returns>
    public MemoryServerless CreateMemoryServerless(AIModel chatModel, AIModel embeddingModel)
    {
        var memoryServerless = new KernelMemoryBuilder()
            .WithQdrantMemoryDb(new QdrantConfig() { Endpoint = _qdrantSettings.Endpoint })
            .WithOpenAITextGeneration(
                new OpenAIConfig() { APIKey = chatModel.ModelKey, TextModel = chatModel.ModelName },
                null,
                new HttpClient(new OpenAICustomHttpClientHandler(chatModel.Endpoint))
            )
            .WithOpenAITextEmbeddingGeneration(
                new OpenAIConfig
                {
                    APIKey = embeddingModel.ModelKey,
                    EmbeddingModel = embeddingModel.ModelName
                },
                null,
                false,
                new HttpClient(new OpenAICustomHttpClientHandler(embeddingModel.Endpoint))
            )
            .Build<MemoryServerless>();

        return memoryServerless;
    }

    /// <summary>
    /// 创建具有fc的kernel
    /// </summary>
    /// <param name="jsFunctionCalls"></param>
    /// <param name="chatModel"></param>
    /// <returns></returns>
    public Kernel CreateFunctionKernel(List<JsFunctionCall>? jsFunctionCalls, AIModel chatModel)
    {
        var kernel = Kernel
            .CreateBuilder()
            .AddOpenAIChatCompletion(chatModel.ModelName, chatModel.ModelKey, chatModel.Endpoint)
            .Build();
        if (jsFunctionCalls == null)
            return kernel;
        foreach (var jsFunctionCall in jsFunctionCalls)
        {
            var function = kernel.CreateFunctionFromMethod(
                async (dynamic value) =>
                {
                    var result = await JsFunctionCallContext.FunctionCallAsync(
                        jsFunctionCall.ScriptContent,
                        jsFunctionCall.MainFuncName,
                        value
                    );
                    return result;
                },
                jsFunctionCall.MainFuncName,
                jsFunctionCall.Description,
                jsFunctionCall
                    .Parameters
                    .Select(
                        x =>
                            new KernelParameterMetadata(x.ParamName)
                            {
                                Name = x.ParamName,
                                Description = x.ParamDescription
                            }
                    )
            );
            kernel
                .Plugins
                .AddFromFunctions(
                    jsFunctionCall.MainFuncName,
                    jsFunctionCall.Description,
                    [ function ]
                );
        }

        return kernel;
    }

    /// <summary>
    /// 只有对话完成的内核
    /// </summary>
    /// <param name="chatModel"></param>
    /// <returns></returns>
    public Kernel CreateFunctionKernel(AIModel chatModel)
    {
        var kernel = Kernel
            .CreateBuilder()
            .AddOpenAIChatCompletion(chatModel.ModelName, chatModel.ModelKey, chatModel.Endpoint)
            .Build();
        return kernel;
    }
}
