using YMKB.UI.APIs.Models;
using YmKB.UI.Models;

namespace YmKB.UI.Services.Contracts;

public interface IAIModelService
{
    Task<List<AIModelDto>> GetAndSearchModelsAsync(
        string? searchKeyword = null,
        AIModelType2 aiModelType = AIModelType2.All
    );
}
