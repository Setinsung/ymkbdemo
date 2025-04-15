namespace YmKB.Domain.Abstractions.Common;

/// <summary>
/// 定义必须关联租户的实体接口。
/// 实现此接口的实体意味着它在业务逻辑中必须与一个特定的租户相关联，
/// 其 TenantId 属性不能为 null。
/// </summary>
public interface IMustHaveTenant
{
    /// <summary>
    /// 获取或设置与实体关联的租户的唯一标识符。
    /// 该属性为必选，实体必须有一个有效的租户 ID。
    /// </summary>
    string TenantId { get; set; }
}

/// <summary>
/// 定义可以关联租户的实体接口。
/// 实现此接口的实体表示它在业务逻辑中可以选择关联一个租户，
/// 其 TenantId 属性可以为 null。
/// </summary>
public interface IMayHaveTenant
{
    /// <summary>
    /// 获取或设置与实体关联的租户的唯一标识符。
    /// 该属性为可选，实体可以不关联任何租户，此时 TenantId 为 null。
    /// </summary>
    string? TenantId { get; set; }
}
