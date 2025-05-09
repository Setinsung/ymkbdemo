using Microsoft.SemanticKernel.Data;
using ReallzeV.SemanticKernel.Plugins.Web.ShaBing;

namespace Microsoft.SemanticKernel;

/// <summary>
/// 用于向 <see cref="IKernelBuilder"/> 注册 <see cref="ITextSearch"/> 的扩展方法。
/// </summary>
public static class ShaBingWebKernelBuilderExtensions
{
    /// <summary>
    /// 使用指定的服务 ID 注册一个 <see cref="ITextSearch"/> 实例。
    /// </summary>
    /// <param name="builder">要在其上注册 <see cref="ITextSearch"/> 的 <see cref="IKernelBuilder"/>。</param>
    /// <param name="options">创建 <see cref="ShaBingTextSearch"/> 时使用的 <see cref="ShaBingTextSearchOptions"/> 实例。</param>
    /// <param name="serviceId">用作服务键的可选服务 ID。</param>
    public static IKernelBuilder AddShaBingSearch(
        this IKernelBuilder builder,
        ShaBingTextSearchOptions? options = null,
        string? serviceId = default
    )
    {
        builder.Services.AddShaBingSearch(options, serviceId);
        return builder;
    }
}
