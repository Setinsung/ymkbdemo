using Mediator;

namespace YmKB.Domain.Abstractions.Events;


/// <summary>
/// 表示领域事件的抽象基类。
/// 该类实现了 INotification 接口，用于在领域驱动设计中表示发生的事件。
/// </summary>
public abstract class DomainEvent : INotification
{
    /// <summary>
    /// 获取或设置一个值，该值指示此领域事件是否已发布。
    /// 如果事件已发布，该属性为 true；否则为 false。
    /// </summary>
    public bool IsPublished { get; set; }

    /// <summary>
    /// 获取事件发生的日期和时间，以协调世界时 (UTC) 表示。
    /// 该属性在类实例化时会自动设置为当前的 UTC 时间，且只能在派生类中进行保护式设置。
    /// </summary>
    public DateTimeOffset DateOccurred { get; protected set; } = DateTimeOffset.UtcNow;
}