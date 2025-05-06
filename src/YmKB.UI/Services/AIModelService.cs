using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using YmKB.UI.Models;
using YmKB.UI.Services.Contracts;

namespace YmKB.UI.Services;

public class AIModelService : IAIModelService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public AIModelService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };
        _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
    public async Task<List<AIModel>> GetAndSearchAllModelsAsync(string? searchKeyword = null, AIModelTypeQuery aiModelType = AIModelTypeQuery.All)
    {
        // var result = await _httpClient.GetFromJsonAsync<List<AIModel>>(
        //     $"AIModels/search?searchKeyword={searchKeyword}&aiModelType={aiModelType}",
        //     _jsonSerializerOptions
        // );
        // return result ?? [];
        
        var response = await _httpClient.GetAsync($"AIModels/search?searchKeyword={searchKeyword}&aiModelType={aiModelType}");
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException(content);
        }
        var result = JsonSerializer.Deserialize<List<AIModel>>(content, _jsonSerializerOptions);
        return result;
    }
}
