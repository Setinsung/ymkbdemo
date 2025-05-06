using Microsoft.Kiota.Abstractions;
using OneOf;
using YMKB.UI.APIs.Models;
using YmKB.UI.Services.JsInterops;

namespace YmKB.UI.Services;

/// <summary>
/// 提供一个用于与 API 交互的服务代理，集成了缓存和错误处理机制。
/// </summary>
/// <param name="logger">用于记录错误和信息的日志实例。</param>
/// <param name="cache">用于本地缓存 API 响应的 IndexedDbCache 实例。</param>
public class ApiClientServiceProxy(ILogger<ApiClientServiceProxy> logger, IndexedDbCache cache)
{
    /// <summary>
    /// 从缓存中检索数据，或者通过提供的工厂函数获取数据，并使用可选的标签和过期时间存储它。
    /// </summary>
    /// <typeparam name="TResponse">响应数据的类型。</typeparam>
    /// <param name="cacheKey">用于标识缓存数据的键。</param>
    /// <param name="factory">如果缓存中没有数据，则用于获取数据的工厂函数。</param>
    /// <param name="tags">可选参数，用于关联缓存数据的标签。</param>
    /// <param name="expiration">可选参数，缓存数据的过期时间。</param>
    /// <returns>缓存的数据或新获取的数据。</returns>
    public async Task<TResponse?> QueryAsync<TResponse>(
        string cacheKey,
        Func<Task<TResponse?>> factory,
        string[]? tags = null,
        TimeSpan? expiration = null
    )
    {
        cacheKey = $"{cacheKey}";
        return await cache.GetOrSetAsync(
            IndexedDbCache.DATABASENAME,
            cacheKey,
            factory,
            tags,
            expiration
        );
    }

    /// <summary>
    /// 删除与特定标签相关联的缓存数据。
    /// </summary>
    /// <param name="tags">要删除的缓存数据所关联的标签。</param>
    public async Task ClearCache(string[] tags)
    {
        await cache.DeleteDataByTagsAsync(IndexedDbCache.DATABASENAME, tags);
    }

    /// <summary>
    /// 使用错误处理机制封装 API 调用，返回值可能是响应结果、验证问题或通用的错误详情。
    /// </summary>
    /// <typeparam name="TResponse">响应数据的类型。</typeparam>
    /// <param name="apiCall">要执行的 API 调用函数。</param>
    /// <returns>API 调用的结果或错误详情。</returns>
    public async Task<
        OneOf<TResponse, HttpValidationProblemDetails, ProblemDetails>
    > ExecuteAsync<TResponse>(Func<Task<TResponse>> apiCall)
    {
        try
        {
            var result = await apiCall();
            return result;
        }
        catch (HttpValidationProblemDetails ex)
        {
            logger.LogError(ex, ex.Message);
            return ex;
        }
        catch (ProblemDetails ex)
        {
            logger.LogError(ex, ex.Message);
            return ex;
        }
        catch (ApiException ex)
        {
            logger.LogError(ex, ex.Message);
            return new ProblemDetails
            {
                Title = ex.Message,
                Detail = ex.InnerException?.Message ?? ex.Message,
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return new ProblemDetails
            {
                Title = ex.Message,
                Detail = ex.InnerException?.Message ?? ex.Message,
            };
        }
    }
}
