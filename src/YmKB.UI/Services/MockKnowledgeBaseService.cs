using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YmKB.UI.Models;

namespace YmKB.UI.Services;

public class MockKnowledgeBaseService : IKnowledgeBaseService
{
    private static readonly List<KnowledgeBase> _knowledgeBases =
        new()
        {
            new KnowledgeBase
            {
                Name = "test",
                Description = "test",
                ConversationModel = "【OpenAI】deepseek-rt-清血版",
                EmbeddingModel = "【OpenAI】text-embedding-ada-002",
                SegmentTokens = 699,
                LineTokens = 299,
                OverlapTokens = 99,
                EnableOcr = false,
                DocumentCount = 1,
                SegmentCount = 0
            },
            new KnowledgeBase
            {
                Name = "test222",
                Description = "test222",
                ConversationModel = "【OpenAI】deepseek-rt-清血版",
                EmbeddingModel = "【OpenAI】text-embedding-ada-002",
                SegmentTokens = 699,
                LineTokens = 299,
                OverlapTokens = 99,
                EnableOcr = false,
                DocumentCount = 2,
                SegmentCount = 2
            }
        };

    public Task<KnowledgeBase> CreateKnowledgeBaseAsync(KnowledgeBase knowledgeBase)
    {
        _knowledgeBases.Add(knowledgeBase);
        return Task.FromResult(knowledgeBase);
    }

    public Task<bool> DeleteKnowledgeBaseAsync(string id)
    {
        var kb = _knowledgeBases.FirstOrDefault(x => x.Id == id);
        if (kb != null)
        {
            _knowledgeBases.Remove(kb);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<KnowledgeBase> GetKnowledgeBaseAsync(string id)
    {
        var kb = _knowledgeBases.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(kb);
    }

    public Task<List<KnowledgeBase>> GetKnowledgeBasesAsync()
    {
        return Task.FromResult(_knowledgeBases);
    }

    public Task<KnowledgeBase> UpdateKnowledgeBaseAsync(KnowledgeBase knowledgeBase)
    {
        var index = _knowledgeBases.FindIndex(x => x.Id == knowledgeBase.Id);
        if (index != -1)
        {
            _knowledgeBases[index] = knowledgeBase;
        }
        return Task.FromResult(knowledgeBase);
    }
}
