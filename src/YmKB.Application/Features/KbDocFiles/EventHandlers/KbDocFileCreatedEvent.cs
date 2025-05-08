using Mediator;
using Microsoft.Extensions.Logging;
using YmKB.Application.Features.KbDocFiles.EventHandlers;
using YmKB.Domain.Abstractions.Events;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.KbDocFiles.EventHandlers;

public class KbDocFileCreatedEvent(KbDocFile item) : CommonDomainEvent<KbDocFile>(item);



// public class KbDocFileCreatedEventHandler(ILogger<KbDocFileCreatedEventHandler> logger)
//     : INotificationHandler<KbDocFileCreatedEvent>
// {
//     public ValueTask Handle(KbDocFileCreatedEvent notification, CancellationToken cancellationToken)
//     {
//         logger.LogInformation($"!!!{notification.Item.Id} KbDocFile 进入后台量化队列!!!");
//         await QuantitativeBackgroundService.AddKbDocFileAsync(notification.Item);
//         return ValueTask.CompletedTask;
//     }
// }