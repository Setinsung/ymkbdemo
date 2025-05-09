using System.Text.Json.Serialization;

namespace ReallzeV.ShaBingSearch;

/// <summary>
/// 定义与查询相关的网页。
/// </summary>
public sealed class ShaBingWebPage
{
    /// <summary>
    /// 唯一标识符。
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// 网页名称。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 网页显示 URL。
    /// </summary>
    [JsonPropertyName("displayUrl")]
    public string? DisplayUrl { get; set; }

    /// <summary>
    /// 网页的 URL。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 描述网页内容的网页文本片段。
    /// </summary>
    [JsonPropertyName("snippet")]
    public string? Snippet { get; set; }

    /// <summary>
    /// 网页所在网站名称
    /// </summary>
    [JsonPropertyName("siteName")]
    public string? SiteName { get; set; }

    /// <summary>
    /// 网页图标url
    /// </summary>
    [JsonPropertyName("siteIcon")]
    public string? SiteIcon { get; set; }

    /// <summary>
    /// 网页展示图片url列表
    /// </summary>
    [JsonPropertyName("siteImage")]
    public List<string> SiteImage { get; set; } = [];

    /// <summary>
    /// 网页的时间。
    /// </summary>
    /// <remarks>
    /// 日期格式为 YYYY-MM-DDTHH:MM:SS。例如，2015-04-13T05:23:39。
    /// </remarks>
    [JsonPropertyName("dateLastCrawled")]
    public string? DateLastCrawled { get; set; }
}
