using YmKB.Domain.Abstractions.Common;

namespace YmKB.Domain.Abstractions.SystemEntities;

/// <summary>
/// 表示租户的类，实现了 IEntity 接口，使用字符串作为主键类型。
/// 租户通常代表系统中的一个独立组织或用户群体，拥有独立的资源和配置。
/// </summary>
public class Tenant : IEntity<string>
{
    /// <summary>
    /// 获取或设置租户的名称。
    /// 该名称用于标识租户，在系统中通常是唯一的。
    /// 若未设置名称，该值为 null。
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置租户的描述信息。
    /// 该描述用于详细说明租户的相关情况，可包含租户的业务范围、特点等信息。
    /// 若未设置描述，该值为 null。
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 获取或设置租户的唯一标识符。
    /// 默认情况下，使用 Guid 的版本 7 生成一个新的唯一标识符。
    /// 该标识符用于在系统中唯一标识一个租户。
    /// </summary>
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
}