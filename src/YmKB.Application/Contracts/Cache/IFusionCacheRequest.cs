using Mediator;

namespace YmKB.Application.Contracts.Cache;
/// <summary>
/// 表示支持使用 FusionCache 缓存的请求。
/// </summary>
/// <typeparam name="TResponse">响应的类型。</typeparam>
public interface IFusionCacheRequest<TResponse> : IRequest<TResponse>
{
    /// <summary>
    /// 获取请求的缓存键。
    /// </summary>
    string CacheKey => string.Empty;

    /// <summary>
    /// 获取与缓存条目关联的标签。
    /// </summary>
    IEnumerable<string>? Tags { get; }
}