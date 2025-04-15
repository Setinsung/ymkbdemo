namespace YmKB.Domain.Abstractions.Events;

public abstract class CommonDomainEvent<T> : DomainEvent
{
    protected CommonDomainEvent(T item)
    {
        Item = item;
    }
    public T Item { get; }
}