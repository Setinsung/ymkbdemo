using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace YmKB.Infrastructure.Extensions;

public static class SerilogExtensions
{
    /// <summary>
    /// 为 <see cref="WebApplicationBuilder"/> 注册 Serilog 日志记录配置。
    /// 此方法设置 Serilog 的调试日志，从应用程序配置中读取日志配置，并设置不同命名空间的最小日志级别。
    /// 同时，它还添加了日志上下文的丰富信息，如 UTC 时间和用户信息，并将日志异步写入文件和控制台。
    /// </summary>
    /// <param name="builder">要注册 Serilog 的 <see cref="WebApplicationBuilder"/> 实例。</param>
    public static void RegisterSerilog(this WebApplicationBuilder builder)
    {
        // 启用 Serilog 的调试日志，将调试信息输出到控制台
        Serilog.Debugging.SelfLog.Enable(Console.WriteLine);
        builder
            .Host
            .UseSerilog(
                (context, configuration) =>
                    configuration
                        .MinimumLevel.Debug()
                        // 从应用程序配置中读取日志配置
                        .ReadFrom
                        .Configuration(context.Configuration)
                        // 设置不同命名空间的最小日志级别
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                        // .MinimumLevel
                        // .Override("Serilog", LogEventLevel.Information)
                        .MinimumLevel
                        .Override("Microsoft.EntityFrameworkCore.AddOrUpdate", LogEventLevel.Error)
                        .MinimumLevel
                        .Override("ZiggyCreatures.Caching.Fusion.FusionCache", LogEventLevel.Information)
                        // 从日志上下文中丰富日志信息
                        .Enrich
                        .FromLogContext()
                        // 添加 UTC 时间到日志信息中
                        .Enrich
                        .WithUtcTime()
                        // 添加用户信息到日志信息中
                        .Enrich
                        .WithUserInfo()
                        // 异步将日志写入文件，每天滚动一个新文件
                        .WriteTo
                        .Async(
                            wt => wt.File("./log/log-.txt", rollingInterval: RollingInterval.Day)
                        )
                        // 异步将日志输出到控制台
                        .WriteTo
                        .Async(
                            wt =>
                                wt.Console(
                                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {ClientIp}] {Message:lj}{NewLine}{Exception}"
                                )
                        )
            );
    }

    /// <summary>
    /// 向日志配置中添加 UTC 时间丰富器。
    /// </summary>
    /// <param name="enrichmentConfiguration">日志丰富配置。</param>
    /// <returns>包含 UTC 时间丰富器的日志配置。</returns>
    private static LoggerConfiguration WithUtcTime(
        this LoggerEnrichmentConfiguration enrichmentConfiguration
    )
    {
        return enrichmentConfiguration.With<UtcTimestampEnricher>();
    }

    /// <summary>
    /// 向日志配置中添加用户信息丰富器。
    /// </summary>
    /// <param name="enrichmentConfiguration">日志丰富配置。</param>
    /// <returns>包含用户信息丰富器的日志配置。</returns>
    private static LoggerConfiguration WithUserInfo(
        this LoggerEnrichmentConfiguration enrichmentConfiguration
    )
    {
        return enrichmentConfiguration.With<UserInfoEnricher>();
    }
}

/// <summary>
/// 用于在日志事件中添加 UTC 时间戳的丰富器。
/// </summary>
internal class UtcTimestampEnricher : ILogEventEnricher
{
    /// <summary>
    /// 向日志事件中添加 UTC 时间戳属性。
    /// </summary>
    /// <param name="logEvent">要丰富的日志事件。</param>
    /// <param name="pf">用于创建日志属性的工厂。</param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory pf)
    {
        logEvent.AddOrUpdateProperty(
            pf.CreateProperty("TimeStamp", logEvent.Timestamp.UtcDateTime)
        );
    }
}

/// <summary>
/// 用于在日志事件中添加用户信息的丰富器。
/// </summary>
internal class UserInfoEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// 使用默认的 <see cref="HttpContextAccessor"/> 初始化 <see cref="UserInfoEnricher"/> 类的新实例。
    /// </summary>
    public UserInfoEnricher()
        : this(new HttpContextAccessor()) { }

    /// <summary>
    /// 使用指定的 <see cref="IHttpContextAccessor"/> 初始化 <see cref="UserInfoEnricher"/> 类的新实例。
    /// </summary>
    /// <param name="httpContextAccessor">用于访问 HTTP 上下文的访问器。</param>
    public UserInfoEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// 向日志事件中添加用户信息属性，如用户名、客户端 IP、客户端代理、活动 ID 和父活动 ID。
    /// </summary>
    /// <param name="logEvent">要丰富的日志事件。</param>
    /// <param name="propertyFactory">用于创建日志属性的工厂。</param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        // 获取用户名
        var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "";
        // 获取请求头
        var headers = _httpContextAccessor.HttpContext?.Request?.Headers;
        // 获取客户端 IP
        var clientIp =
            headers != null && headers.ContainsKey("X-Forwarded-For")
                ? headers["X-Forwarded-For"].ToString().Split(',').First().Trim()
                : _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "";
        // 获取客户端代理
        var clientAgent =
            headers != null && headers.ContainsKey("User-Agent")
                ? headers["User-Agent"].ToString()
                : "";
        // 获取活动信息
        var activity = _httpContextAccessor
            .HttpContext
            ?.Features
            .Get<IHttpActivityFeature>()
            ?.Activity;
        if (activity != null)
        {
            // 添加活动 ID 到日志事件
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ActivityId", activity.Id));
            // 添加父活动 ID 到日志事件
            logEvent.AddPropertyIfAbsent(
                propertyFactory.CreateProperty("ParentId", activity.ParentId)
            );
        }
        // 添加用户名到日志事件
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserName", userName));
        // 添加客户端 IP 到日志事件
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ClientIP", clientIp));
        // 添加客户端代理到日志事件
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ClientAgent", clientAgent));
    }
}
