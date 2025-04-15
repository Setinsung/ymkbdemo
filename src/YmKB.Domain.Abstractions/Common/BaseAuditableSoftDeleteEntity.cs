namespace YmKB.Domain.Abstractions.Common;

/// <summary>
/// 可审计且支持软删除的实体抽象基类。
/// 该类继承自 BaseAuditableEntity，意味着它具备基本的审计功能，能够记录实体的创建和修改信息。
/// 同时，它实现了 ISoftDelete 接口，支持软删除操作，即数据不会真正从数据库中移除，而是标记为已删除。
/// </summary>
public abstract class BaseAuditableSoftDeleteEntity : BaseAuditableEntity, ISoftDelete
{
    /// <summary>
    /// 获取或设置数据被标记为删除的日期和时间。
    /// 若实体未被删除，该值为 null；若已删除，则记录删除操作发生的时间。
    /// </summary>
    public DateTime? Deleted { get; set; }

    /// <summary>
    /// 获取或设置执行删除操作的用户的标识。
    /// 若实体未被删除，该值为 null；若已删除，则记录执行删除操作的用户的相关标识信息。
    /// </summary>
    public string? DeletedBy { get; set; }
}