using YmKB.Domain.Abstractions.Events;
using YmKB.Domain.Entities.Trial;

namespace YmKB.Application.Features.Products.EventHandlers;
/// <summary>
/// Represents an event triggered when a product is updated.
/// Purpose:
/// 1. To signal that a product has been updated.
/// 2. Used in the domain event notification mechanism to inform subscribers about the updated product details.
/// </summary>
public class ProductUpdatedEvent : DomainEvent
{
    /// <summary>
    /// Constructor to initialize the event and pass the updated product instance.
    /// </summary>
    /// <param name="item">The updated product instance.</param>
    public ProductUpdatedEvent(Product item)
    {
        Item = item; // Assigns the provided product instance to the read-only property
    }

    /// <summary>
    /// Gets the product instance associated with the event.
    /// </summary>
    public Product Item { get; }
}
/*
public class ProductUpdatedEventHandler : INotificationHandler<ProductUpdatedEvent>
{
    private readonly ILogger<ProductUpdatedEventHandler> _logger;

    public ProductUpdatedEventHandler(
        ILogger<ProductUpdatedEventHandler> logger
    )
    {
        _logger = logger;
    }

    public async Task Handle(ProductUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} in {ElapsedMilliseconds} ms", notification.GetType().Name, notification, _timer.ElapsedMilliseconds);
    }
}
*/
