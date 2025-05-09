using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using YmKB.Domain.Entities;

namespace YmKB.Application.Contracts;

public interface IAIKernelCreateService
{
    /// <summary>
    /// 创建用于处理向量的内存服务
    /// </summary>
    /// <param name="searchClientConfig"></param>
    /// <param name="maxTokensPerParagraph"></param>
    /// <param name="overlappingTokens"></param>
    /// <param name="chatModel"></param>
    /// <param name="embeddingModel"></param>
    /// <returns></returns>
    MemoryServerless CreateMemoryServerless(
        SearchClientConfig searchClientConfig,
        int maxTokensPerParagraph,
        int overlappingTokens,
        AIModel chatModel,
        AIModel embeddingModel
    );

    /// <summary>
    /// 仅用于操作的内存服务，可用于单独向量搜索
    /// </summary>
    /// <returns></returns>
    MemoryServerless CreateMemoryServerless(AIModel chatModel, AIModel embeddingModel);

    /// <summary>
    /// 创建具有fc的kernel
    /// </summary>
    /// <param name="jsFunctionCalls"></param>
    /// <param name="chatModel"></param>
    /// <returns></returns>
    Kernel CreateFunctionKernel(List<JsFunctionCall>? jsFunctionCalls, AIModel chatModel);

    /// <summary>
    /// 只有对话完成的内核
    /// </summary>
    /// <param name="chatModel"></param>
    /// <returns></returns>
    Kernel CreateFunctionKernel(AIModel chatModel);
}
