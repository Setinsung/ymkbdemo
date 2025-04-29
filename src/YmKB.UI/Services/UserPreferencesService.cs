using YmKB.UI.Contracts;
using YmKB.UI.Models;

namespace YmKB.UI.Services;

public class UserPreferencesService : IUserPreferencesService
{
    private readonly IStorageService _localStorage;
    private const string Key = "_userPreferences";

    public UserPreferencesService(IStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task SaveUserPreferences(UserPreferences userPreferences)
    {
        await _localStorage.SetItemAsync(Key, userPreferences);
    }

    public async Task<UserPreferences> LoadUserPreferences()
    {
        return await _localStorage.GetItemAsync<UserPreferences>(Key) ?? new UserPreferences();
    }
}
