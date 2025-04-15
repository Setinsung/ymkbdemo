using YmKB.Domain.Abstractions.Events;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.AIModels.EventHandlers;

public class AIModelUpdatedEvent : DomainEvent
{
    public AIModelUpdatedEvent(AIModel item)
    {
        Item = item;
    }
    public AIModel Item { get; }
}