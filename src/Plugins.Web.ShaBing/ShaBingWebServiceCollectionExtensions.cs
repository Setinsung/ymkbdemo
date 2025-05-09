using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Data;
using ReallzeV.SemanticKernel.Plugins.Web.ShaBing;

namespace Microsoft.SemanticKernel;

/// <summary>
/// 用于向 <see cref="IServiceCollection"/> 注册 <see cref="ITextSearch"/> 的扩展方法。
/// </summary>
public static class ShaBingWebServiceCollectionExtensions
{
    /// <summary>
    /// 使用指定的服务 ID 注册一个 <see cref="ITextSearch"/> 实例。
    /// </summary>
    /// <param name="services">要在其上注册 <see cref="ITextSearch"/> 的 <see cref="IServiceCollection"/>。</param>
    /// <param name="options">创建 <see cref="ShaBingTextSearch"/> 时使用的 <see cref="ShaBingTextSearchOptions"/> 实例。</param>
    /// <param name="serviceId">用作服务键的可选服务 ID。</param>
    public static IServiceCollection AddShaBingSearch(
        this IServiceCollection services,
        ShaBingTextSearchOptions? options = null,
        string? serviceId = default
    )
    {
        services.AddKeyedSingleton<ITextSearch>(
            serviceId,
            (sp, obj) =>
            {
                var selectedOptions = options ?? sp.GetService<ShaBingTextSearchOptions>();
                return new ShaBingTextSearch(selectedOptions);
            }
        );

        return services;
    }
}
