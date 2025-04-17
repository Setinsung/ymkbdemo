using Microsoft.KernelMemory;

namespace YmKB.Infrastructure.Services;

public class KbMemoryCreateService
{
    public MemoryServerless CreateMemoryServerless(SearchClientConfig searchClientConfig,
        int maxTokensPerLine,
        int maxTokensPerParagraph,
        int overlappingTokens,
        string? chatModel, string? embeddingModel)
    {
        throw new NotImplementedException();
    }
}