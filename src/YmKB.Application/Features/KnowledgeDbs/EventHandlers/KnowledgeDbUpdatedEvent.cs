using YmKB.Domain.Abstractions.Events;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.KnowledgeDbs.EventHandlers;

public class KnowledgeDbUpdatedEvent(KnowledgeDb item) : CommonDomainEvent<KnowledgeDb>(item);
