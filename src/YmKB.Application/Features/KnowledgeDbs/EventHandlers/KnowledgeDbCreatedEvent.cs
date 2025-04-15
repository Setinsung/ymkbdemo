using YmKB.Domain.Abstractions.Events;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.KnowledgeDbs.EventHandlers;

public class KnowledgeDbCreatedEvent(KnowledgeDb item) : CommonDomainEvent<KnowledgeDb>(item);