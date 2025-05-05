namespace YmKB.UI.Services.Contracts;

public interface IStorageService
{
    /// <summary>
    /// 异步地从存储中检索一个项目。
    /// </summary>
    /// <typeparam name="T">要检索的项目的类型。</typeparam>
    /// <param name="key">要检索的项目的键。</param>
    /// <returns>一个 <see cref="ValueTask{T}"/> 表示异步操作。任务结果包含检索到的项目，如果项目不存在则为 null。</returns>
    ValueTask<T?> GetItemAsync<T>(string key);

    /// <summary>
    /// 异步地从存储中移除一个项目。
    /// </summary>
    /// <param name="key">要移除的项目的键。</param>
    /// <returns>一个 <see cref="ValueTask"/> 表示异步操作。</returns>
    ValueTask RemoveItemAsync(string key);

    /// <summary>
    /// 异步地在存储中设置一个项目。
    /// </summary>
    /// <typeparam name="T">要设置的项目的类型。</typeparam>
    /// <param name="key">要设置的项目的键。</param>
    /// <param name="value">要设置的项目的值。</param>
    /// <returns>一个 <see cref="ValueTask"/> 表示异步操作。</returns>
    ValueTask SetItemAsync<T>(string key, T value);
}