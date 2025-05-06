using YmKB.UI.Models;

namespace YmKB.UI.Services.Contracts;

public interface IAIModelService
{
    Task<List<AIModel>> GetAndSearchAllModelsAsync(string? searchKeyword = null, AIModelTypeQuery aiModelType = AIModelTypeQuery.All);
}