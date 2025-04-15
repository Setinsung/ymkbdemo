using Microsoft.EntityFrameworkCore.ChangeTracking;
using YmKB.Domain.Abstractions.Common;
using YmKB.Domain.Abstractions.Identities;

namespace YmKB.Domain.Abstractions.SystemEntities;

/// <summary>
/// 表示审计追踪记录的类，实现了 IEntity 接口，使用字符串作为主键类型。
/// 该类用于记录系统中各种操作的审计信息，便于后续的审计和追溯。
/// </summary>
public class AuditTrail : IEntity<string>
{
    /// <summary>
    /// 获取或设置审计追踪记录的唯一标识符。
    /// 该标识符用于在系统中唯一标识一条审计追踪记录。
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// 获取或设置执行操作的用户的 ID。
    /// 如果操作是由用户执行的，该属性将包含用户的唯一标识符；否则为 null。
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// 获取或设置执行操作的用户对象。
    /// 这是一个虚拟属性，用于关联执行操作的 ApplicationUser 对象。
    /// 如果操作是由用户执行的，该属性将引用对应的用户对象；否则为 null。
    /// </summary>
    public virtual ApplicationUser? Owner { get; set; }

    /// <summary>
    /// 获取或设置审计操作的类型。
    /// 该类型指示了操作是创建、更新、删除还是无操作。
    /// </summary>
    public AuditType AuditType { get; set; }

    /// <summary>
    /// 获取或设置受影响的数据库表名。
    /// 如果操作涉及数据库表，该属性将包含表的名称；否则为 null。
    /// </summary>
    public string? TableName { get; set; }

    /// <summary>
    /// 获取或设置操作发生的日期和时间。
    /// 该时间记录了操作执行的具体时刻。
    /// </summary>
    public DateTime DateTime { get; set; }

    /// <summary>
    /// 获取或设置操作前的旧值。
    /// 这是一个字典，键为属性名，值为属性的旧值。
    /// 如果操作没有涉及旧值的变化，该属性为 null。
    /// </summary>
    public Dictionary<string, object?>? OldValues { get; set; }

    /// <summary>
    /// 获取或设置操作后的新值。
    /// 这是一个字典，键为属性名，值为属性的新值。
    /// 如果操作没有涉及新值的变化，该属性为 null。
    /// </summary>
    public Dictionary<string, object?>? NewValues { get; set; }

    /// <summary>
    /// 获取或设置受影响的列名列表。
    /// 该列表包含了操作中受影响的列的名称。
    /// 如果操作没有影响任何列，该属性为 null。
    /// </summary>
    public List<string>? AffectedColumns { get; set; }

    /// <summary>
    /// 获取或设置受影响记录的主键信息。
    /// 这是一个字典，键为主键属性名，值为主键属性值。
    /// 默认初始化为空字典。
    /// </summary>
    public Dictionary<string, object> PrimaryKey { get; set; } = new();

    /// <summary>
    /// 获取临时属性的列表。
    /// 这些临时属性通常用于存储一些临时的审计相关信息。
    /// 默认初始化为空列表。
    /// </summary>
    public List<PropertyEntry> TemporaryProperties { get; } = [];

    /// <summary>
    /// 获取一个值，指示是否存在临时属性。
    /// 如果临时属性列表不为空，则返回 true；否则返回 false。
    /// </summary>
    public bool HasTemporaryProperties => TemporaryProperties.Count != 0;

    /// <summary>
    /// 获取或设置调试视图信息。
    /// 该信息可用于调试和查看详细的审计操作信息。
    /// 如果没有调试信息，该属性为 null。
    /// </summary>
    public string? DebugView { get; set; }

    /// <summary>
    /// 获取或设置操作过程中出现的错误信息。
    /// 如果操作执行过程中出现错误，该属性将包含错误的详细描述；否则为 null。
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// 表示审计操作类型的枚举。
/// 定义了不同类型的审计操作，用于区分不同的数据库操作。
/// </summary>
public enum AuditType
{
    /// <summary>
    /// 无操作类型。
    /// </summary>
    None,

    /// <summary>
    /// 创建操作类型。
    /// 表示执行了创建新记录的操作。
    /// </summary>
    Create,

    /// <summary>
    /// 更新操作类型。
    /// 表示执行了更新现有记录的操作。
    /// </summary>
    Update,

    /// <summary>
    /// 删除操作类型。
    /// 表示执行了删除现有记录的操作。
    /// </summary>
    Delete
}
