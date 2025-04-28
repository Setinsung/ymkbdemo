using YmKB.Domain.Abstractions.Events;
using YmKB.Domain.Entities;

namespace YmKB.Application.Features.KbDocFiles.EventHandlers;

public class KbDocFileDeletedEvent(KbDocFile item) : CommonDomainEvent<KbDocFile>(item);