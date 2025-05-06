using System.Text.Json;
using Microsoft.JSInterop;

namespace YmKB.UI.Services.JsInterops;

/// <summary>
/// 提供用于与 IndexedDB 进行交互以实现缓存目的的方法。
/// </summary>
public sealed class IndexedDbCache
{
    /// <summary>
    /// IndexedDB 数据库的名称。
    /// </summary>
    public const string DATABASENAME = "YMKB.IndexedDB";
    private readonly IJSRuntime _jsRuntime;
    
    public IndexedDbCache(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// 将数据保存到 IndexedDB 中，并可选择设置标签和过期时间。
    /// </summary>
    /// <typeparam name="T">要保存的数据类型。</typeparam>
    /// <param name="dbName">数据库的名称。</param>
    /// <param name="key">用于标识数据的键。</param>
    /// <param name="value">要保存的数据。</param>
    /// <param name="tags">与数据关联的可选标签。</param>
    /// <param name="expiration">数据的可选过期时间。</param>
    public async Task SaveDataAsync<T>(string dbName, string key, T value, string[]? tags = null, TimeSpan? expiration = null)
    {
        var expirationMs = expiration.HasValue ? (int)expiration.Value.TotalMilliseconds : (int?)null;
        await _jsRuntime.InvokeVoidAsync("indexedDbStorage.saveData", dbName, key, value, tags ?? Array.Empty<string>(), expirationMs);
    }

    /// <summary>
    /// 从 IndexedDB 中获取数据，如果数据不存在则设置数据。
    /// </summary>
    /// <typeparam name="T">数据的类型。</typeparam>
    /// <param name="dbName">数据库的名称。</param>
    /// <param name="key">用于标识数据的键。</param>
    /// <param name="factory">如果数据不存在，用于创建数据的工厂函数。</param>
    /// <param name="tags">与数据关联的可选标签。</param>
    /// <param name="expiration">数据的可选过期时间。</param>
    /// <returns>缓存中的数据或新创建的数据。</returns>
    public async Task<T> GetOrSetAsync<T>(string dbName, string key, Func<Task<T>> factory, string[]? tags = null, TimeSpan? expiration = null)
    {
        var existingData = await GetDataAsync<T>(dbName, key);
        if (existingData != null)
        {
            return existingData;
        }

        var newData = await factory();
        await SaveDataAsync(dbName, key, newData, tags, expiration);
        return newData;
    }

    /// <summary>
    /// 通过键从 IndexedDB 中获取数据。
    /// </summary>
    /// <typeparam name="T">数据的类型。</typeparam>
    /// <param name="dbName">数据库的名称。</param>
    /// <param name="key">用于标识数据的键。</param>
    /// <returns>缓存中的数据。</returns>
    public async Task<T> GetDataAsync<T>(string dbName, string key)
    {
        return await _jsRuntime.InvokeAsync<T>("indexedDbStorage.getData", dbName, key);
    }

    /// <summary>
    /// 通过标签从 IndexedDB 中获取所有数据。
    /// </summary>
    /// <typeparam name="T">数据的类型。</typeparam>
    /// <param name="dbName">数据库的名称。</param>
    /// <param name="tags">用于过滤数据的标签。</param>
    /// <returns>数据的键值对字典。</returns>
    public async Task<Dictionary<string, T>> GetDataByTagsAsync<T>(string dbName, string[] tags)
    {
        var results = await _jsRuntime.InvokeAsync<List<Dictionary<string, object>>>("indexedDbStorage.getDataByTags", dbName, tags);

        return results.ToDictionary(
            result => result["key"].ToString(),
            result =>
            {
                var jsonElement = result["value"];
                return JsonSerializer.Deserialize<T>(jsonElement.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        );
    }

    /// <summary>
    /// 通过键从 IndexedDB 中删除特定数据。
    /// </summary>
    /// <param name="dbName">数据库的名称。</param>
    /// <param name="key">用于标识数据的键。</param>
    public async Task DeleteDataAsync(string dbName, string key)
    {
        await _jsRuntime.InvokeVoidAsync("indexedDbStorage.deleteData", dbName, key);
    }

    /// <summary>
    /// 通过标签从 IndexedDB 中删除所有数据。
    /// </summary>
    /// <param name="dbName">数据库的名称。</param>
    /// <param name="tags">用于过滤数据的标签。</param>
    public async Task DeleteDataByTagsAsync(string dbName, string[] tags)
    {
        await _jsRuntime.InvokeVoidAsync("indexedDbStorage.deleteDataByTags", dbName, tags);
    }

    /// <summary>
    /// 清除 IndexedDB 存储中的所有数据。
    /// </summary>
    /// <param name="dbName">数据库的名称。</param>
    public async Task ClearDataAsync(string dbName)
    {
        await _jsRuntime.InvokeVoidAsync("indexedDbStorage.clearData", dbName);
    }
}
