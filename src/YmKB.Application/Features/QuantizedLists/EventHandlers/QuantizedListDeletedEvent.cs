using YmKB.Domain.Abstractions.Events;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.QuantizedLists.EventHandlers;

public class QuantizedListDeletedEvent(QuantizedList item) : CommonDomainEvent<QuantizedList>(item);