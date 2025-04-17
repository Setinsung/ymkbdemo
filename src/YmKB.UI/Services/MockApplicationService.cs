using YmKB.UI.Models;

namespace YmKB.UI.Services;

public class MockApplicationService : IApplicationService
{
    private static readonly List<Application> _applications =
        new()
        {
            new Application
            {
                Name = "豆包推理模型",
                Description = "豆包推理模型",
                Type = "对话应用",
                ConversationModel = "Doubao-1.5-thinking-pro",
                EmbeddingModel = "text-embedding-ada-002",
                Temperature = 0.7,
                TopP = 0.95
            },
            new Application
            {
                Name = "DeepSeek-R1",
                Description = "DeepSeek-R1",
                Type = "对话应用",
                ConversationModel = "deepseek-r1-满血版",
                EmbeddingModel = "text-embedding-ada-002",
                Temperature = 0.7,
                TopP = 0.95
            },
            new Application
            {
                Name = "我想试试",
                Description = "我想试试",
                Type = "知识库",
                ConversationModel = "doubao-1.5-lite-32k-250115",
                EmbeddingModel = "text-embedding-ada-002",
                Temperature = 0.7,
                TopP = 0.95
            }
        };

    public Task<Application> CreateApplicationAsync(Application application)
    {
        _applications.Add(application);
        return Task.FromResult(application);
    }

    public Task<bool> DeleteApplicationAsync(string id)
    {
        var app = _applications.FirstOrDefault(x => x.Id == id);
        if (app != null)
        {
            _applications.Remove(app);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<Application?> GetApplicationAsync(string id)
    {
        var app = _applications.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(app);
    }

    public Task<(List<Application> Items, int TotalCount)> GetApplicationsAsync(
        string? searchText,
        int page,
        int pageSize
    )
    {
        var query = _applications.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(
                x =>
                    x.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                    || x.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase)
            );
        }

        var totalCount = query.Count();
        var items = query
            .OrderByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult((items, totalCount));
    }

    public Task<Application> UpdateApplicationAsync(Application application)
    {
        var index = _applications.FindIndex(x => x.Id == application.Id);
        if (index != -1)
        {
            _applications[index] = application;
        }
        return Task.FromResult(application);
    }
}
