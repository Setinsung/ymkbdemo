using YmKB.Domain.Abstractions.Events;
using YmKB.Domain.Entities.Trial;

namespace YmKB.Application.Features.Products.EventHandlers;
/// <summary>
/// Represents an event triggered when a product is deleted.
/// Purpose:
/// 1. To signal the deletion of a product.
/// 2. Used in the domain event notification mechanism to inform subscribers about the deleted product.
/// </summary>
public class ProductDeletedEvent : DomainEvent
{
    /// <summary>
    /// Constructor to initialize the event and pass the deleted product instance.
    /// </summary>
    /// <param name="item">The deleted product instance.</param>
    public ProductDeletedEvent(Product item)
    {
        Item = item; // Assigns the provided product instance to the read-only property
    }

    /// <summary>
    /// Gets the product instance associated with the event.
    /// </summary>
    public Product Item { get; }
}


/*
public class ProductDeletedEventHandler : INotificationHandler<ProductDeletedEvent>
{
    private readonly ILogger<ProductDeletedEventHandler> _logger;

    public ProductDeletedEventHandler(
        ILogger<ProductDeletedEventHandler> logger
    )
    {
        _logger = logger;
    }

    public async Task Handle(ProductDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} in {ElapsedMilliseconds} ms", notification.GetType().Name, notification, _timer.ElapsedMilliseconds);
    }
}
*/
