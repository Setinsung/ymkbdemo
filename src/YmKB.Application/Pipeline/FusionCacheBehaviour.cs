using Mediator;
using Microsoft.Extensions.Logging;
using YmKB.Application.Contracts.Cache;
using ZiggyCreatures.Caching.Fusion;

namespace YmKB.Application.Pipeline;

/// <summary>
/// 使用 FusionCache 处理请求的 Pipeline 行为。
/// </summary>
/// <typeparam name="TRequest">请求的类型。</typeparam>
/// <typeparam name="TResponse">响应的类型。</typeparam>
public class FusionCacheBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IFusionCacheRequest<TResponse>
{
    private readonly IFusionCache _fusionCache;
    private readonly ILogger<FusionCacheBehaviour<TRequest, TResponse>> _logger;

    public FusionCacheBehaviour(
        IFusionCache fusionCache,
        ILogger<FusionCacheBehaviour<TRequest, TResponse>> logger
    )
    {
        _fusionCache = fusionCache;
        _logger = logger;
    }

    /// <summary>
    /// 通过尝试从缓存中检索响应，或者如果未找到，则调用下一个处理程序来处理请求。
    /// </summary>
    /// <param name="request">请求实例。</param>
    /// <param name="next">下一个处理程序委托。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>响应实例。</returns>
    public async ValueTask<TResponse> Handle(
        TRequest request,
        MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation(
            "Handling request of type {RequestType} with cache key {CacheKey}",
            nameof(request),
            request.CacheKey
        );
        var response = await _fusionCache
            .GetOrSetAsync<TResponse>(
                request.CacheKey,
                async (_, token) => await next(request, token), // 异步工厂方法，缓存中不存在时调用方法获取响应
                tags: request.Tags,
                token: cancellationToken
            )
            .ConfigureAwait(false);

        return response;
    }
}
