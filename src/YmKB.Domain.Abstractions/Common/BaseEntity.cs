using System.ComponentModel.DataAnnotations.Schema;
using YmKB.Domain.Abstractions.Events;

namespace YmKB.Domain.Abstractions.Common;
/// <summary>
/// 表示所有实体的基类。
/// 该类实现了 IEntity 接口，使用字符串作为主键类型。
/// </summary>
public abstract class BaseEntity : IEntity<string>
{
    /// <summary>
    /// 存储该实体产生的领域事件的列表。
    /// 该列表在对象初始化时创建。
    /// </summary>
    private readonly List<DomainEvent> _domainEvents = new List<DomainEvent>();

    /// <summary>
    /// 获取或设置实体的唯一标识符。
    /// 默认情况下，使用 Guid 的版本 7 生成一个新的唯一标识符。
    /// </summary>
    public virtual string Id { get; set; } = Guid.CreateVersion7().ToString();

    /// <summary>
    /// 获取与该实体关联的只读领域事件集合。
    /// 此属性在数据库映射时会被忽略。
    /// </summary>
    [NotMapped]
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// 向实体添加一个新的领域事件。
    /// </summary>
    /// <param name="domainEvent">要添加的领域事件。</param>
    public void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// 从实体中移除指定的领域事件。
    /// </summary>
    /// <param name="domainEvent">要移除的领域事件。</param>
    public void RemoveDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// 清除与该实体关联的所有领域事件。
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}