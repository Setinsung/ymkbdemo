namespace YmKB.Domain.Abstractions.Common;

/// <summary>
/// 可审计实体的抽象基类，继承自 BaseEntity 并实现了 IAuditableEntity 接口。
/// 该类为实体提供了基础的审计功能，包括记录创建和最后修改的相关信息。
/// </summary>
public abstract class BaseAuditableEntity : BaseEntity, IAuditableEntity
{
    /// <summary>
    /// 获取或设置实体的创建日期和时间。
    /// 若实体已创建，此属性记录创建操作的发生时间；若未创建，则值为 null。
    /// 此属性为虚属性，允许派生类进行重写。
    /// </summary>
    public virtual DateTime? Created { get; set; }

    /// <summary>
    /// 获取或设置创建实体的用户标识。
    /// 若实体已创建，此属性记录执行创建操作的用户标识信息；若未创建，则值为 null。
    /// 此属性为虚属性，允许派生类进行重写。
    /// </summary>
    public virtual string? CreatedBy { get; set; }

    /// <summary>
    /// 获取或设置实体最后一次修改的日期和时间。
    /// 若实体有过修改，此属性记录最后一次修改操作的发生时间；若未修改过，则值为 null。
    /// 此属性为虚属性，允许派生类进行重写。
    /// </summary>
    public virtual DateTime? LastModified { get; set; }

    /// <summary>
    /// 获取或设置最后一次修改实体的用户标识。
    /// 若实体有过修改，此属性记录执行最后一次修改操作的用户标识信息；若未修改过，则值为 null。
    /// 此属性为虚属性，允许派生类进行重写。
    /// </summary>
    public virtual string? LastModifiedBy { get; set; }
}