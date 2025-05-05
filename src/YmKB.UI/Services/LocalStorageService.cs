using Blazored.LocalStorage;
using YmKB.UI.Services.Contracts;

namespace YmKB.UI.Services;

/// <summary>
/// 用于与本地存储交互的服务。
/// </summary>
public class LocalStorageService : IStorageService
{
    private readonly ILocalStorageService _localStorageService;

    public LocalStorageService(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }
    
    public ValueTask<T?> GetItemAsync<T>(string key)
    {
        return _localStorageService.GetItemAsync<T>(key);
    }

    public ValueTask RemoveItemAsync(string key)
    {
        return _localStorageService.RemoveItemAsync(key);
    }

    public ValueTask SetItemAsync<T>(string key, T value)
    {
        return _localStorageService.SetItemAsync(key, value);
    }

    public ValueTask<bool> ContainKeyAsync(string key)
    {
        return _localStorageService.ContainKeyAsync(key);
    }
}

