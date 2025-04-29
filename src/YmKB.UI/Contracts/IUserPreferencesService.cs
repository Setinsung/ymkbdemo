using YmKB.UI.Models;

namespace YmKB.UI.Contracts;

/// <summary>
/// 用于管理用偏好
/// </summary>
public interface IUserPreferencesService
{
    /// <summary>
    /// 将 UserPreferences 保存在本地存储中。
    /// </summary>
    /// <param name="userPreferences">要保存在本地存储中的 userPreferences。</param>
    /// <returns>表示异步作的任务。</returns>
    public Task SaveUserPreferences(UserPreferences userPreferences);

    /// <summary>
    /// 从本地存储加载 UserPreferences。
    /// </summary>
    /// <returns>UserPreferences 对象。未找到设置时为 null。</returns>
    public Task<UserPreferences> LoadUserPreferences();
}
