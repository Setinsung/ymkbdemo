using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Data;

namespace ReallzeV.SemanticKernel.Plugins.Web.ShaBing;

/// <summary>
/// 用于构造 <see cref="ShaBingTextSearch"/> 实例的配置
/// </summary>
public sealed class ShaBingTextSearchOptions
{
    /// <summary>
    /// Bing 搜索服务的 URI 端点。
    /// </summary>
    public Uri? Host { get; init; } = null;

    /// <summary>
    /// 等待网页加载的超时时间。
    /// </summary>
    public int? Timeout { get; set; } = null;

    /// <summary>
    /// 限制单个网站的snippet长度。
    /// </summary>
    public int? SnippetMaxLength { get; set; } = null;

    /// <summary>
    /// 用于日志记录的 <see cref="ILoggerFactory"/>。如果为 null，则不进行日志记录。
    /// </summary>
    public ILoggerFactory? LoggerFactory { get; init; } = null;

    /// <summary>
    /// 将搜索结果映射为 <see cref="string"/> 的 <see cref="ITextSearchStringMapper" /> 实例
    /// </summary>
    public ITextSearchStringMapper? StringMapper { get; init; } = null;

    /// <summary>
    /// 将搜索结果映射为 <see cref="TextSearchResult"/> 的 <see cref="ITextSearchResultMapper" /> 实例
    /// </summary>
    public ITextSearchResultMapper? ResultMapper { get; init; } = null;
}