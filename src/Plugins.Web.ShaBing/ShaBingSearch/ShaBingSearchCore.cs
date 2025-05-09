using System.Globalization;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Web;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Playwright;
using Microsoft.SemanticKernel.Data;

namespace ReallzeV.ShaBingSearch;

/// <summary>
/// Sha Bing 搜索核心类
/// </summary>
public class ShaBingSearchCore : IDisposable
{
    private IPlaywright _playwright;
    private IBrowser _browser;
    private readonly int _timeout = 2500;
    private readonly int _snippetMaxLength = 300;
    private readonly string _host = "https://cn.bing.com";
    private readonly ILogger _logger;
    private readonly bool _debug;

    /// <summary>
    ///
    /// </summary>
    /// <param name="host">Sha Bing 搜索实例的 URI。默认为 "https://cn.bing.com"。</param>
    /// <param name="timeout">网页加载的超时时间。</param>
    /// <param name="snippetMaxLength">网站的snippet最大长度。</param>
    /// <param name="logger">用于日志记录</param>
    /// <param name="debug">debug 设置为 true时，关闭浏览器 headless 模式进行查看</param>
    public ShaBingSearchCore(
        string? host = null,
        int? timeout = null,
        int? snippetMaxLength = null,
        ILogger? logger = null,
        bool debug = false
    )
    {
        _host = host ?? _host;
        _timeout = timeout ?? _timeout;
        _snippetMaxLength = snippetMaxLength ?? _snippetMaxLength;
        _logger = logger ?? NullLogger.Instance;
        _debug = debug;
        _playwright = Playwright.CreateAsync().GetAwaiter().GetResult();
        _browser = _playwright
            .Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = !_debug })
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    /// 执行 Sha Bing 搜索查询并返回结果。
    /// </summary>
    /// <param name="query">要搜索的内容。</param>
    /// <param name="searchOptions">搜索选项。</param>
    /// <param name="cancellationToken">用于监视取消请求的 <see cref="CancellationToken"/>。默认值为 <see cref="CancellationToken.None"/>。</param>
    public async Task<ShaBingSearchResponse<ShaBingWebPage>?> ExecuteSearchAsync(
        string query,
        TextSearchOptions searchOptions,
        CancellationToken cancellationToken = default
    )
    {
        var results = new List<ShaBingWebPage>();
        long? totalEstimatedMatches = null;

        if (string.IsNullOrEmpty(query) || searchOptions.Top <= 0)
            return null;

        var page = await _browser.NewPageAsync();

        int skipCount = searchOptions.Skip;
        int currentPage = 0;

        try
        {
            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                var queryParams = new Dictionary<string, string>
                {
                    { "q", query },
                    { "FPIG", Guid.NewGuid().ToString("N") },
                    { "first", (currentPage * 10).ToString() },
                    { "FORM", "PERE1" },
                };
                string url = $"{_host}/search?{UrlEncode(queryParams)}";

                var document = await PageLoadUrlAsync(page, url);
                if (document is null)
                    break;

                var bResults = document.QuerySelector("#b_results");
                if (bResults == null)
                {
                    _logger.LogInformation("No search results found.");
                    break;
                }
                // 第一页查询总数，同时可能有ans解答的项
                if (currentPage == 0)
                {
                    totalEstimatedMatches = ParseTotalCountFromText(
                        document.QuerySelector("#b_tween_searchResults span")?.TextContent.Trim()
                    );
                    var webPage = ParseTopAnsLi(bResults.QuerySelector("li.b_ans.b_top"));
                    if (webPage is not null)
                    {
                        if (skipCount >= 1)
                            skipCount--;
                        else
                        {
                            results.Add(webPage);
                            if (results.Count >= searchOptions.Top)
                                break;
                        }
                    }
                }

                // 正常解析
                var resultItems = bResults.QuerySelectorAll("li.b_algo");
                if (resultItems.Length == 0)
                {
                    _logger.LogInformation("No result items on current page.");
                    break;
                }
                if (skipCount < 10)
                {
                    foreach (var li in resultItems)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        if (skipCount > 0)
                            skipCount--;
                        else
                        {
                            var algoWebPage = ParseAlgoLi(li, results.Count);
                            results.Add(algoWebPage);
                            if (results.Count >= searchOptions.Top)
                                break;
                        }
                    }
                    if (results.Count >= searchOptions.Top)
                        break;
                    currentPage++;
                }
                else
                {
                    currentPage = skipCount / 10;
                    skipCount = skipCount % 10;
                }
            } while (results.Count < searchOptions.Top);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex}");
        }
        finally
        {
            await page.CloseAsync();
        }

        var response = new ShaBingSearchResponse<ShaBingWebPage>
        {
            Type = "SearchResponse",
            QueryContext = new ShaBingQueryContext { OriginalQuery = query },
            WebPages = new ShaBingWebPages<ShaBingWebPage>
            {
                Id = Guid.NewGuid().ToString(),
                TotalEstimatedMatches = totalEstimatedMatches ?? 0,
                WebSearchUrl = $"{_host}/search?q={WebUtility.UrlEncode(query)}",
                Value = results,
            },
        };
        _logger.LogInformation("\n----查询输入 " + query);
        _logger.LogInformation("\n----查询条件 " + JsonSerializer.Serialize(searchOptions));
        _logger.LogInformation("\n----搜索内容 " + JsonSerializer.Serialize(response));
        return response;
    }

    private async Task<IHtmlDocument?> PageLoadUrlAsync(IPage page, string url)
    {
        string? content = null;
        try
        {
            await page.GotoAsync(
                url,
                new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle, Timeout = _timeout }
            );
            content = await page.ContentAsync();
            if (content.Length < 100)
            {
                await page.ReloadAsync(
                    new PageReloadOptions
                    {
                        WaitUntil = WaitUntilState.NetworkIdle,
                        Timeout = _timeout,
                    }
                );
                content = await page.ContentAsync();
            }
        }
        catch (TimeoutException)
        {
            content = await page.ContentAsync();
        }
        var parser = new HtmlParser();
        var document = await parser.ParseDocumentAsync(content);
        return document;
    }

    private ShaBingWebPage ParseAlgoLi(IElement li, int count)
    {
        var urlNode = li.QuerySelector("div.b_tpcn a");
        var title = li.QuerySelector("h2 a")?.TextContent.Trim();
        var iurl = urlNode?.GetAttribute("href")?.Trim();
        //if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(iurl))
        //    continue;

        var siteName = urlNode?.GetAttribute("aria-label")?.Trim();

        var iconUrl = li.QuerySelector("div.wr_fav div img")?.GetAttribute("src")?.Trim();

        var abstractText = li.QuerySelector("div.b_caption p.b_lineclamp2")?.TextContent.Trim();
        if (string.IsNullOrEmpty(abstractText))
        {
            abstractText = li.QuerySelector("div.b_imgcap_altitle p.b_lineclamp3")
                ?.TextContent.Trim();
        }
        if (string.IsNullOrEmpty(abstractText))
        {
            abstractText =
                li.QuerySelector("div.b_caption p.b_lineclamp3")?.TextContent.Trim()
                + li.QuerySelector("div.b_caption div.b_factrow div.b_vlist2col")
                    ?.TextContent.Trim();
        }
        if (string.IsNullOrEmpty(abstractText))
        {
            abstractText = li.QuerySelector("div.b_algoQuizGoBig")?.TextContent.Trim();
        }
        if (string.IsNullOrEmpty(abstractText))
            abstractText = li.QuerySelector("div.tab-content")?.TextContent.Trim();

        var (dateLastCrawled, trimmedAbstract) = ParseDateAndTrimText(abstractText);

        List<string> imgs = [];
        var multiImageUl = li.QuerySelectorAll("ul.b_hList li");
        if (multiImageUl.Length != 0)
        {
            foreach (var imgLi in multiImageUl)
            {
                var a = imgLi.QuerySelector("a");
                var imageUrl = ParseImageUrlInAnchor(a);
                if (!string.IsNullOrEmpty(imageUrl))
                    imgs.Add(imageUrl);
            }
        }
        else
        {
            var imageUrl = li.QuerySelector("div.mc_vtvc_con_rc div.b_canvas div.cico img")
                ?.GetAttribute("src")
                ?.Trim();
            if (string.IsNullOrEmpty(imageUrl))
            {
                imageUrl = ParseImageUrlInAnchor(
                    li.QuerySelector("div.b_imgcap_altitle div.b_imagePair div.inner a")
                );
            }
            if (!string.IsNullOrEmpty(imageUrl))
                imgs.Add(imageUrl);
        }

        return new ShaBingWebPage
        {
            Id = (count + 1).ToString(),
            Name = title,
            DisplayUrl = iurl,
            Url = iurl,
            Snippet = LimitSnippetLength(trimmedAbstract),
            SiteIcon = iconUrl,
            SiteName = siteName,
            SiteImage = imgs,
            DateLastCrawled = dateLastCrawled,
        };
    }

    private ShaBingWebPage? ParseTopAnsLi(IElement? li)
    {
        if (li is null)
            return null;
        var urlNode = li.QuerySelector("h2 a");
        var title = urlNode?.TextContent?.Trim();
        var iurl = urlNode?.GetAttribute("href")?.Trim();
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(iurl))
            return null;
        var siteName = li.QuerySelector("div.b_attribution cite")?.TextContent?.Trim();
        var iconUrl = li.QuerySelector("div.b_imagePair img")?.GetAttribute("src")?.Trim();
        if (string.IsNullOrEmpty(iconUrl))
            iconUrl = li.QuerySelector("div.rdtopattr div.cico img")?.GetAttribute("src")?.Trim();
        var abstractText = li.QuerySelector("div.qna_body")?.TextContent?.Trim();
        if (string.IsNullOrEmpty(abstractText))
            abstractText = li.QuerySelector("div.rd_card_ml")?.TextContent?.Trim();
        if (string.IsNullOrEmpty(abstractText))
            abstractText =
                li.QuerySelector("div.df_con div.rwrl")?.TextContent?.Trim()
                + li.QuerySelector("div.df_con div.rch-cap-cntr")?.TextContent?.Trim();

        return new ShaBingWebPage
        {
            Id = "1",
            Name = title,
            DisplayUrl = iurl,
            Url = iurl,
            Snippet = LimitSnippetLength(abstractText),
            SiteIcon = iconUrl,
            SiteName = siteName,
        };
    }

    private long? ParseTotalCountFromText(string? input)
    {
        if (input is null)
            return null;
        string cleanNumber = input.Replace("约 ", "").Replace(" 个结果", "").Replace(",", "");
        if (long.TryParse(cleanNumber, out long number))
            return number;
        return null;
    }

    private string? ParseImageUrlInAnchor(IElement? a)
    {
        var originUrl = a?.GetAttribute("aria-label")?.Trim();
        if (string.IsNullOrEmpty(originUrl))
            originUrl = a?.GetAttribute("href")?.Trim();
        if (originUrl is null)
            return null;
        string decodedUrl = WebUtility.HtmlDecode(originUrl);
        var uri = new Uri(_host + decodedUrl);
        var queryParams = HttpUtility.ParseQueryString(uri.Query);
        return queryParams["mediaurl"];
    }

    private (string? ParsedDate, string? TrimmedInput) ParseDateAndTrimText(string? input)
    {
        if (input is null)
            return (null, null);

        string trimmedInput = input;
        int dotIndex = input.IndexOf(" · ");
        string? datePart;
        if (dotIndex != -1 && dotIndex < 20)
        {
            datePart = input[..dotIndex].Trim();
            trimmedInput = input[(dotIndex + 3)..];
        }
        else
        {
            return (null, trimmedInput);
        }
        if (datePart.EndsWith("天前") || datePart.EndsWith("天之前"))
        {
            int days = int.Parse(datePart.Replace("天前", "").Replace("天之前", "").Trim());
            return (
                DateOnly.FromDateTime(DateTime.Today).AddDays(-days).ToString("yyyy-MM-dd"),
                trimmedInput
            );
        }
        if (datePart.EndsWith("之前"))
        {
            return (DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd"), trimmedInput);
        }
        if (
            DateOnly.TryParseExact(
                datePart,
                "yyyy年M月d日",
                null,
                DateTimeStyles.None,
                out var parsedDate
            )
        )
        {
            return (parsedDate.ToString("yyyy-MM-dd"), trimmedInput);
        }
        return (null, trimmedInput);
    }

    private string UrlEncode(Dictionary<string, string> parameters)
    {
        return string.Join(
            "&",
            parameters.Select(kvp =>
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"
            )
        );
    }

    private string? LimitSnippetLength(string? input)
    {
        if (input is null)
            return null;
        int maxLength = _snippetMaxLength;
        if (input.Length <= maxLength)
            return input;
        else
            return input[..maxLength];
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _browser?.CloseAsync()?.GetAwaiter().GetResult();
        _browser?.DisposeAsync().GetAwaiter().GetResult();
        _playwright?.Dispose();
    }
}
