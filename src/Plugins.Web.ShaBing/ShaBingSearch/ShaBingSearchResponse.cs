using System.Text.Json.Serialization;

namespace ReallzeV.ShaBingSearch;

/// <summary>
/// Sha Bing 搜索响应。
/// </summary>
public sealed class ShaBingSearchResponse<T>
{
    /// <summary>
    /// 类型提示，设置为 SearchResponse。
    /// </summary>
    [JsonPropertyName("_type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Bing 用于请求的查询字符串。
    /// </summary>
    [JsonPropertyName("queryContext")]
    public ShaBingQueryContext? QueryContext { get; set; }

    /// <summary>
    /// 一个可空的 WebAnswer 对象，包含 Web 搜索 API 响应数据。
    /// </summary>
    [JsonPropertyName("webPages")]
    public ShaBingWebPages<T>? WebPages { get; set; }
}

/// <summary>
/// Sha Bing 用于请求的查询字符串。
/// </summary>
public sealed class ShaBingQueryContext
{
    /// <summary>
    /// 请求中指定的查询字符串。
    /// </summary>
    [JsonPropertyName("originalQuery")]
    public string OriginalQuery { get; set; } = string.Empty;

    /// <summary>
    /// 错误修改查询字符串，无用
    /// </summary>
    [JsonPropertyName("alteredQuery")]
    public string? AlteredQuery { get; set; }
}

/// <summary>
/// 与搜索查询相关的网页列表。
/// </summary>
public sealed class ShaBingWebPages<T>
{
    /// <summary>
    /// 唯一标识网页搜索结果的 ID。
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 与查询相关的估计网页数量。可能无法获取成功
    /// </summary>
    [JsonPropertyName("totalEstimatedMatches")]
    public long TotalEstimatedMatches { get; set; }

    /// <summary>
    /// 所请求网页搜索URL。
    /// </summary>
    [JsonPropertyName("webSearchUrl")]
    public string WebSearchUrl { get; set; } = string.Empty;

    /// <summary>
    /// 与查询相关的网页列表。
    /// </summary>
    [JsonPropertyName("value")]
    public IList<T>? Value { get; set; }
}
