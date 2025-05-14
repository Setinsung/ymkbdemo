using YmKB.Domain.Abstractions.Events;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.JsFunctionCalls.EventHandlers;

public class JsFunctionCallDeletedEvent(JsFunctionCall item) : CommonDomainEvent<JsFunctionCall>(item);
