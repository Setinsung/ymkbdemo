using YmKB.UI.Models;

namespace YmKB.UI.Services;
public interface IAIModelService
{
    Task<List<AIModel>> GetAllModelsAsync();
    Task<AIModel> GetModelByIdAsync(string id);
    Task<AIModel> CreateModelAsync(AIModel model);
    Task<AIModel> UpdateModelAsync(string id, AIModel model);
    Task DeleteModelAsync(string id);
}
