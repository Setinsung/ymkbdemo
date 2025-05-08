using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Serilog;

namespace YmKB.Infrastructure.Handlers;

/// <summary>
/// 用于请求重定向到自定义的endpoint
/// </summary>
public partial class OpenAICustomHttpClientHandler : HttpClientHandler
{
    private readonly string _baseUrl;
    private readonly string _complectionPath = "/chat/completions";
    private readonly string _embeddingPath = "/embeddings";
    private readonly bool _isLog;

    [GeneratedRegex(@"/v\d$")]
    private static partial Regex VersionPattern();

    private static readonly string[] UrlSources =  [ "api.openai.com", "openai.azure.com" ];

    /// <summary>
    /// 使用指定的模型URL初始化<see cref="OpenAICustomHttpClientHandler"/>类的新实例。
    /// </summary>
    /// <param name="baseUrl">用于OpenAI或Azure OpenAI请求的基础URL。</param>
    public OpenAICustomHttpClientHandler(string baseUrl, bool isLog = false)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("模型URL不能为空或空白。", nameof(baseUrl));
        // 检查是否末尾携带了版本号，如果没有则默认加上 /v1
        if (!VersionPattern().IsMatch(baseUrl))
        {
            baseUrl += "/v1";
        }
        _baseUrl = baseUrl;
        _isLog = isLog;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        // 替换请求路径
        if (request.RequestUri is not null && UrlSources.Contains(request.RequestUri.Host))
        {
            if (request.RequestUri.PathAndQuery.EndsWith("completions"))
            {
                request.RequestUri = new Uri(_baseUrl + _complectionPath);
            }
            else if (request.RequestUri.PathAndQuery.EndsWith("embeddings"))
            {
                request.RequestUri = new Uri(_baseUrl + _embeddingPath);
            }
            else
            {
                throw new NotImplementedException($"未实现此分支。请求路径：{request.RequestUri}");
            }
        }

        // 记录请求 URI 和方法
        ConsoleWrite($"Request: {request.Method} {request.RequestUri}");
        // 检查是否是包含 JSON 内容的 POST 请求
        if (request.Method == HttpMethod.Post && request.Content != null)
        {
            // 从 Authorization 标头获取 Bearer 令牌
            if (request.Headers.Authorization?.Scheme == "Bearer")
            {
                ConsoleWrite($"Bearer Token: {request.Headers.Authorization.Parameter}");
            }

            // 将 POST 请求的内容读取为字符串
            var requestBody = await request.Content.ReadAsStringAsync();
            ConsoleWrite($"Request Body: {requestBody}");
        }

        // 编码转换
        // var mediaType = request.Content.Headers.ContentType.MediaType;
        // request.Content = new StringContent(
        //     Unescape(await request.Content.ReadAsByteArrayAsync(cancellationToken)),
        //     Encoding.UTF8,
        //     mediaType
        // );

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
        // Read the content of the POST response as a string
        var responseBody = await response.Content.ReadAsStringAsync();
        ConsoleWrite($"Response Body: {responseBody}");
        return response;
    }

    /// <summary>
    /// 请求的编码可能导致AI无法响应想要的信息，因此转为UTF8
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static string Unescape(byte[] stream)
    {
        try
        {
            var str = JsonSerializer.Serialize(
                JsonSerializer.Deserialize(
                    stream,
                    typeof(object),
                    new JsonSerializerOptions()
                    {
                        Encoder = System
                            .Text
                            .Encodings
                            .Web
                            .JavaScriptEncoder
                            .UnsafeRelaxedJsonEscaping,
                    }
                ),
                new JsonSerializerOptions()
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                }
            );
            return str;
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
            return "";
        }
    }

    private void ConsoleWrite(string? value)
    {
        if (_isLog)
            Log.Information(value);
        else
            return;
    }
}
