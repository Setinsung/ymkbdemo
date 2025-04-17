using YmKB.Domain.Abstractions.Events;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.KbApps.EventHandlers;

public class KbAppCreatedEvent(KbApp item) : CommonDomainEvent<KbApp>(item);
