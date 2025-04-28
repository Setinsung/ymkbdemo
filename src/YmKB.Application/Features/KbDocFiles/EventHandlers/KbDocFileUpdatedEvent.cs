using YmKB.Domain.Abstractions.Events;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.KbDocFiles.EventHandlers;

public class KbDocFileUpdatedEvent(KbDocFile item) : CommonDomainEvent<KbDocFile>(item);
