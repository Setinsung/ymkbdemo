using Mediator;

namespace YmKB.Application.Contracts.Cache;

/// <summary>
/// 表示刷新 FusionCache 中的缓存项的请求。
/// </summary>
/// <typeparam name="TResponse">响应的类型</typeparam>
public interface IFusionCacheRefreshRequest<TResponse> : IRequest<TResponse>
{
    /// <summary>
    /// 获取与请求关联的缓存键。
    /// </summary>
    string CacheKey => string.Empty;

    /// <summary>
    /// 获取与缓存条目关联的标签。
    /// </summary>
    IEnumerable<string>? Tags { get; }
}
