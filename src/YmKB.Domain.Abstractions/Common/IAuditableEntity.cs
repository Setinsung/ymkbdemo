namespace YmKB.Domain.Abstractions.Common;

/// <summary>
/// 定义可审计实体的接口。
/// 实现此接口的实体将具备审计功能，能够记录实体的创建和最后修改的相关信息。
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// 获取或设置实体的创建日期和时间。
    /// 如果实体还未被创建，该值为 null；若已创建，则记录创建操作发生的时间。
    /// </summary>
    DateTime? Created { get; set; }

    /// <summary>
    /// 获取或设置创建实体的用户的标识。
    /// 如果实体还未被创建，该值为 null；若已创建，则记录执行创建操作的用户的相关标识信息。
    /// </summary>
    string? CreatedBy { get; set; }

    /// <summary>
    /// 获取或设置实体最后一次被修改的日期和时间。
    /// 如果实体还未被修改过，该值为 null；若有修改，则记录最后一次修改操作发生的时间。
    /// </summary>
    DateTime? LastModified { get; set; }

    /// <summary>
    /// 获取或设置最后一次修改实体的用户的标识。
    /// 如果实体还未被修改过，该值为 null；若有修改，则记录执行最后一次修改操作的用户的相关标识信息。
    /// </summary>
    string? LastModifiedBy { get; set; }
}