using YmKB.UI.Models;

namespace YmKB.UI.Services;

public interface IKnowledgeBaseService
{
    Task<List<KnowledgeBase>> GetKnowledgeBasesAsync();
    Task<KnowledgeBase> GetKnowledgeBaseAsync(string id);
    Task<bool> DeleteKnowledgeBaseAsync(string id);
    Task<KnowledgeBase> CreateKnowledgeBaseAsync(KnowledgeBase knowledgeBase);
    Task<KnowledgeBase> UpdateKnowledgeBaseAsync(KnowledgeBase knowledgeBase);
}
