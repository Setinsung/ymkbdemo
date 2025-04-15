using YmKB.Domain.Abstractions.Events;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.AIModels.EventHandlers;

public class AIModelCreatedEvent :DomainEvent
{
    public AIModelCreatedEvent(AIModel item)
    {
        Item = item;
    }
    public AIModel Item { get; }
}