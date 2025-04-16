using Mediator;
using Microsoft.Extensions.Logging;
using YmKB.Domain.Abstractions.Events;
using YmKB.Domain.Entities.Trial;

namespace YmKB.Application.Features.Products.EventHandlers;
/// <summary>
/// 表示在创建产品时触发的事件。
/// 1. 发出产品创建的信号。
/// 2. 用于域事件通知机制，将产品详细信息传递给订阅者。
/// </summary>
public class ProductCreatedEvent : DomainEvent
{
    /// <summary>
    /// Constructor 初始化事件并传递创建的 product 实例。
    /// </summary>
    /// <param name="item">创建的 product 实例。</param>
    public ProductCreatedEvent(Product item)
    {
        Item = item; // 将提供的产品实例分配给 read-only 属性
    }

    /// <summary>
    /// 获取与事件关联的 product 实例。
    /// </summary>
    public Product Item { get; }
}

public class ProductCreatedEventHandler : INotificationHandler<ProductCreatedEvent>
{
    private readonly ILogger<ProductCreatedEventHandler> _logger;

    public ProductCreatedEventHandler(
        ILogger<ProductCreatedEventHandler> logger
    )
    {
        _logger = logger;
    }


    public ValueTask Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        // _logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} in {ElapsedMilliseconds} ms", notification.GetType().Name, notification, _timer.ElapsedMilliseconds);
        _logger.LogInformation("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        _logger.LogInformation("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        _logger.LogInformation("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        _logger.LogInformation("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        return ValueTask.CompletedTask;
    }
}
