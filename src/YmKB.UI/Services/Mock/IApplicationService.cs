using YmKB.UI.Models;

namespace YmKB.UI.Services;

public interface IApplicationService
{
    Task<(List<Application> Items, int TotalCount)> GetApplicationsAsync(
        string? searchText,
        int page,
        int pageSize
    );
    Task<Application?> GetApplicationAsync(string id);
    Task<bool> DeleteApplicationAsync(string id);
    Task<Application> CreateApplicationAsync(Application application);
    Task<Application> UpdateApplicationAsync(Application application);
}
