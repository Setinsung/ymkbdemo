using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using YMKB.UI.APIs.Models;
using YmKB.UI.Models;
using YmKB.UI.Services.Contracts;

namespace YmKB.UI.Services;

public class AIModelService : IAIModelService
{
    public Task<List<AIModelDto>> GetAndSearchModelsAsync(string? searchKeyword = null, AIModelType2 aiModelType = AIModelType2.All)
    {
        throw new NotImplementedException();
    }
}
