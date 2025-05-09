using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
using ReallzeV.ShaBingSearch;

namespace ReallzeV.SemanticKernel.Plugins.Web.ShaBing;

/// <summary>
/// Sha Bing API 连接器。
/// </summary>
public class ShaBingConnector : IWebSearchEngineConnector
{
    private readonly ILogger _logger;
    private readonly ShaBingSearchCore _shaBingSearchCore;

    /// <summary>
    /// 初始化 <see cref="ShaBingConnector"/> 类的新实例。
    /// </summary>
    /// <param name="host">Sha Bing 搜索实例的 URI。默认为 "https://cn.bing.com"。</param>
    /// <param name="timeout">网页加载的超时时间。</param>
    /// <param name="snippetMaxLength">网站的snippet最大长度。</param>
    /// <param name="loggerFactory">用于日志记录的 <see cref="ILoggerFactory"/>。如果为 null，则不进行日志记录。</param>
    public ShaBingConnector(
        Uri? host = null,
        int? timeout = null,
        int? snippetMaxLength = null,
        ILoggerFactory? loggerFactory = null
    )
    {
        // 创建日志记录器，如果未提供日志工厂，则使用空日志记录器
        this._logger = loggerFactory?.CreateLogger(typeof(BingConnector)) ?? NullLogger.Instance;
        this._shaBingSearchCore = new ShaBingSearchCore(
            host: host?.ToString(),
            timeout: timeout,
            snippetMaxLength: snippetMaxLength,
            logger: _logger
        );
    }

    /// <summary>
    /// 执行一次网络搜索引擎搜索。
    /// </summary>
    /// <param name="query">要搜索的查询内容。</param>
    /// <param name="count">搜索结果的数量。</param>
    /// <param name="offset">要跳过的搜索结果数量。</param>
    /// <param name="cancellationToken">用于监视取消请求的 <see cref="CancellationToken"/>。默认值为 <see cref="CancellationToken.None"/>。</param>
    /// <returns>搜索返回的首批片段内容。</returns>
    public async Task<IEnumerable<T>> SearchAsync<T>(
        string query,
        int count = 1,
        int offset = 0,
        CancellationToken cancellationToken = default
    )
    {
        // 检查 count 参数是否在有效范围内
        if (count is <= 0 or >= 50)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count),
                count,
                $"{nameof(count)} 值必须大于 0 且小于 50。"
            );
        }
        var data = await _shaBingSearchCore.ExecuteSearchAsync(
            query,
            new TextSearchOptions() { Top = count, Skip = offset }
        );

        List<T>? returnValues = null;
        if (data?.WebPages?.Value is not null)
        {
            if (typeof(T) == typeof(string))
            {
                WebPage[]? results = data?.WebPages?.Value.Select(ConvertToWebPage).ToArray();
                returnValues = results?.Select(x => x.Snippet).ToList() as List<T>;
            }
            else if (typeof(T) == typeof(WebPage))
            {
                List<WebPage>? webPages = data?.WebPages?.Value.Select(ConvertToWebPage).ToList();
                returnValues = webPages?.Take(count).ToList() as List<T>;
            }
            else
            {
                throw new NotSupportedException($"Type {typeof(T)} is not supported.");
            }
        }

        return returnValues is null ? []
            : returnValues.Count <= count ? returnValues
            : returnValues.Take(count);
    }

    private WebPage ConvertToWebPage(ShaBingWebPage shaBingWebPage) =>
        new()
        {
            Name = shaBingWebPage.Name ?? string.Empty,
            Url = shaBingWebPage.Url ?? string.Empty,
            Snippet = shaBingWebPage.Snippet ?? string.Empty,
        };
}
