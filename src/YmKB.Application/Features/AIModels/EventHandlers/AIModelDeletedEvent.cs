using YmKB.Domain.Abstractions.Events;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.AIModels.EventHandlers;

public class AIModelDeletedEvent : DomainEvent
{

    public AIModelDeletedEvent(AIModel item)
    {
        Item = item;
    }
    public AIModel Item { get; }
}