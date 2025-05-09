using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel.Data;
using ReallzeV.ShaBingSearch;

namespace ReallzeV.SemanticKernel.Plugins.Web.ShaBing;

/// <summary>
/// 使用 ShaBingSearchCore 的 unofficial Bing Web Search API 执行搜索。
/// </summary>
public class ShaBingTextSearch : ITextSearch
{
    /// <summary>
    /// 初始化 <see cref="ShaBingTextSearch"/> 类的新实例。
    /// </summary>
    /// <param name="options">创建此 <see cref="ShaBingTextSearch"/> 实例时使用的选项。</param>
    public ShaBingTextSearch(ShaBingTextSearchOptions? options = null)
    {
        this._logger =
            options?.LoggerFactory?.CreateLogger(typeof(ShaBingTextSearch)) ?? NullLogger.Instance;
        this._shaBingSearchCore = new ShaBingSearchCore(
            options?.Host?.ToString(),
            options?.Timeout,
            options?.SnippetMaxLength,
            _logger
        );
        this._stringMapper = options?.StringMapper ?? s_defaultStringMapper;
        this._resultMapper = options?.ResultMapper ?? s_defaultResultMapper;
    }

    /// <summary>
    /// 执行与指定查询相关的内容搜索，并返回表示搜索结果的 <see cref="string"/> 值。
    /// </summary>
    /// <param name="query">要搜索的内容。</param>
    /// <param name="searchOptions">执行文本搜索时使用的选项。</param>
    /// <param name="cancellationToken">用于监视取消请求的 <see cref="CancellationToken"/>。默认值为 <see cref="CancellationToken.None"/>。</param>
    public async Task<KernelSearchResults<string>> SearchAsync(
        string query,
        TextSearchOptions? searchOptions,
        CancellationToken cancellationToken = default
    )
    {
        searchOptions ??= new TextSearchOptions();
        ShaBingSearchResponse<ShaBingWebPage>? searchResponse = await _shaBingSearchCore
            .ExecuteSearchAsync(query, searchOptions, cancellationToken)
            .ConfigureAwait(false);
        long? totalCount = searchOptions.IncludeTotalCount
            ? searchResponse?.WebPages?.TotalEstimatedMatches
            : null;

        return new KernelSearchResults<string>(
            this.GetResultsAsStringAsync(searchResponse, cancellationToken),
            totalCount,
            GetResultsMetadata(searchResponse)
        );
        throw new NotImplementedException();
    }

    /// <summary>
    /// 执行与指定查询相关的内容搜索，并返回表示搜索结果的 <see cref="TextSearchResult"/> 值。
    /// </summary>
    /// <param name="query">要搜索的内容。</param>
    /// <param name="searchOptions">执行文本搜索时使用的选项。</param>
    /// <param name="cancellationToken">用于监视取消请求的 <see cref="CancellationToken"/>。默认值为 <see cref="CancellationToken.None"/>。</param>
    public async Task<KernelSearchResults<TextSearchResult>> GetTextSearchResultsAsync(
        string query,
        TextSearchOptions? searchOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        searchOptions ??= new TextSearchOptions();
        var searchResponse = await _shaBingSearchCore
            .ExecuteSearchAsync(query, searchOptions, cancellationToken)
            .ConfigureAwait(false);

        long? totalCount = searchOptions.IncludeTotalCount
            ? searchResponse?.WebPages?.TotalEstimatedMatches
            : null;

        return new KernelSearchResults<TextSearchResult>(
            this.GetResultsAsTextSearchResultAsync(searchResponse, cancellationToken),
            totalCount,
            GetResultsMetadata(searchResponse)
        );
    }

    /// <summary>
    /// 执行与指定查询相关的内容搜索，并返回表示搜索结果的 <see cref="object"/> 值。
    /// </summary>
    /// <param name="query">要搜索的内容。</param>
    /// <param name="searchOptions">执行文本搜索时使用的选项。</param>
    /// <param name="cancellationToken">用于监视取消请求的 <see cref="CancellationToken"/>。默认值为 <see cref="CancellationToken.None"/>。</param>
    public async Task<KernelSearchResults<object>> GetSearchResultsAsync(
        string query,
        TextSearchOptions? searchOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        searchOptions ??= new TextSearchOptions();
        var searchResponse = await this
            ._shaBingSearchCore.ExecuteSearchAsync(query, searchOptions, cancellationToken)
            .ConfigureAwait(false);

        long? totalCount = searchOptions.IncludeTotalCount
            ? searchResponse?.WebPages?.TotalEstimatedMatches
            : null;

        return new KernelSearchResults<object>(
            this.GetResultsAsWebPageAsync(searchResponse, cancellationToken),
            totalCount,
            GetResultsMetadata(searchResponse)
        );
    }

    #region private
    private readonly ShaBingSearchCore _shaBingSearchCore;
    private readonly ILogger _logger;
    private readonly ITextSearchStringMapper _stringMapper;
    private readonly ITextSearchResultMapper _resultMapper;
    private static readonly ITextSearchStringMapper s_defaultStringMapper =
        new DefaultTextSearchStringMapper();
    private static readonly ITextSearchResultMapper s_defaultResultMapper =
        new DefaultTextSearchResultMapper();

    /// <summary>
    /// 将搜索结果作为 <see cref="TextSearchResult"/> 实例返回。
    /// </summary>
    /// <param name="searchResponse">包含与查询匹配的网页的响应。</param>
    /// <param name="cancellationToken">取消令牌</param>
    private async IAsyncEnumerable<string> GetResultsAsStringAsync(
        ShaBingSearchResponse<ShaBingWebPage>? searchResponse,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        // 如果搜索响应为空，或者响应中的网页信息为空，或者网页值列表为空，则结束迭代
        if (
            searchResponse is null
            || searchResponse.WebPages is null
            || searchResponse.WebPages.Value is null
        )
        {
            yield break;
        }

        foreach (var webPage in searchResponse.WebPages.Value)
        {
            // 使用字符串映射器将网页映射为字符串并返回
            yield return this._stringMapper.MapFromResultToString(webPage);
            // 让出当前线程的控制权，允许其他任务执行
            await Task.Yield();
        }
    }

    /// <summary>
    /// 将搜索结果作为 <see cref="TextSearchResult"/> 实例返回。
    /// </summary>
    /// <param name="searchResponse">包含与查询匹配的网页的响应。</param>
    /// <param name="cancellationToken">取消令牌</param>
    private async IAsyncEnumerable<TextSearchResult> GetResultsAsTextSearchResultAsync(
        ShaBingSearchResponse<ShaBingWebPage>? searchResponse,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        if (
            searchResponse is null
            || searchResponse.WebPages is null
            || searchResponse.WebPages.Value is null
        )
        {
            yield break;
        }

        foreach (var webPage in searchResponse.WebPages.Value)
        {
            yield return this._resultMapper.MapFromResultToTextSearchResult(webPage);
            await Task.Yield();
        }
    }

    /// <summary>
    /// 将搜索结果作为 <see cref="ShaBingWebPage"/> 实例返回。
    /// </summary>
    /// <param name="searchResponse">包含与查询匹配的网页的响应。</param>
    /// <param name="cancellationToken">取消令牌</param>
    private async IAsyncEnumerable<object> GetResultsAsWebPageAsync(
        ShaBingSearchResponse<ShaBingWebPage>? searchResponse,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        if (
            searchResponse is null
            || searchResponse.WebPages is null
            || searchResponse.WebPages.Value is null
        )
        {
            yield break;
        }

        foreach (var webPage in searchResponse.WebPages.Value)
        {
            yield return webPage;
            await Task.Yield();
        }
    }

    /// <summary>
    /// 返回结果的元数据。
    /// </summary>
    /// <param name="searchResponse">包含与查询匹配的文档的响应。</param>
    private static Dictionary<string, object?>? GetResultsMetadata(
        ShaBingSearchResponse<ShaBingWebPage>? searchResponse
    )
    {
        // 创建一个字典用于存储结果的元数据
        return new Dictionary<string, object?>()
        {
            // 将修改后的查询添加到元数据字典中，如果搜索响应或查询上下文为空则值为 null
            { "AlteredQuery", searchResponse?.QueryContext?.AlteredQuery },
        };
    }

    /// <summary>
    /// 从 <see cref="ShaBingWebPage"/> 映射到 <see cref="string"/> 的默认实现
    /// </summary>
    private sealed class DefaultTextSearchStringMapper : ITextSearchStringMapper
    {
        /// <inheritdoc />
        public string MapFromResultToString(object result)
        {
            // 检查传入的结果是否为 BingWebPage 类型
            if (result is not ShaBingWebPage webPage)
            {
                throw new ArgumentException("结果必须是 ShaBingWebPage 类型", nameof(result));
            }

            return webPage.Snippet ?? string.Empty;
        }
    }

    /// <summary>
    /// 从 <see cref="ShaBingWebPage"/> 映射到 <see cref="TextSearchResult"/> 的默认实现
    /// </summary>
    private sealed class DefaultTextSearchResultMapper : ITextSearchResultMapper
    {
        /// <inheritdoc />
        public TextSearchResult MapFromResultToTextSearchResult(object result)
        {
            if (result is not ShaBingWebPage webPage)
            {
                throw new ArgumentException("结果必须是 ShaBingWebPage 类型", nameof(result));
            }

            return new TextSearchResult(webPage.Snippet ?? string.Empty)
            {
                Name = webPage.Name,
                Link = webPage.Url,
            };
        }
    }
    #endregion
}
