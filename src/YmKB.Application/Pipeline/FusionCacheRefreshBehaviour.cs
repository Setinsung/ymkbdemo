using Mediator;
using Microsoft.Extensions.Logging;
using YmKB.Application.Contracts.Cache;
using ZiggyCreatures.Caching.Fusion;

namespace YmKB.Application.Pipeline;

/// <summary>
/// 用于处理 FusionCache 刷新请求的管道行为。即请求后清除缓存使下次请求时重新获取。
/// </summary>
/// <typeparam name="TRequest">请求的类型。</typeparam>
/// <typeparam name="TResponse">响应的类型。</typeparam>
public class FusionCacheRefreshBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IFusionCacheRefreshRequest<TResponse>
{
    private readonly IFusionCache _cache;
    private readonly ILogger<FusionCacheRefreshBehaviour<TRequest, TResponse>> _logger;

    public FusionCacheRefreshBehaviour(
        IFusionCache cache,
        ILogger<FusionCacheRefreshBehaviour<TRequest, TResponse>> logger
    )
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// 处理请求并在必要时刷新缓存。
    /// </summary>
    /// <param name="request">请求实例。</param>
    /// <param name="next">管道中的下一个委托。</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>来自管道中下一个委托的响应。</returns>
    public async ValueTask<TResponse> Handle(
        TRequest request,
        MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken
    )
    {
        _logger.LogTrace(
            "Handling request of type {RequestType} with details {@Request}",
            nameof(request),
            request
        );
        var response = await next(request, cancellationToken).ConfigureAwait(false);
        if (!string.IsNullOrEmpty(request.CacheKey))
        {
            await _cache.RemoveAsync(request.CacheKey, token: cancellationToken);
            _logger.LogTrace("Cache key {CacheKey} removed from cache", request.CacheKey);
        }
        if (request.Tags != null && request.Tags.Any())
        {
            foreach (var tag in request.Tags)
            {
                await _cache.RemoveByTagAsync(tag, token: cancellationToken);
                _logger.LogTrace("Cache tag {CacheTag} removed from cache", tag);
            }
        }
        return response;
    }
}
