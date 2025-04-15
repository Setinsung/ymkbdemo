namespace YmKB.Domain.Abstractions.Common;

/// <summary>
/// 定义软删除功能的接口。
/// 实现此接口的类将具备软删除的能力，即数据不会真正从数据库中移除，而是标记为已删除。
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// 获取或设置数据被标记为删除的日期和时间。
    /// 如果数据未被删除，该值为 null；若已删除，则记录删除操作发生的时间。
    /// </summary>
    DateTime? Deleted { get; set; }

    /// <summary>
    /// 获取或设置执行删除操作的用户的标识。
    /// 如果数据未被删除，该值为 null；若已删除，则记录执行删除操作的用户的相关标识信息。
    /// </summary>
    string? DeletedBy { get; set; }
}