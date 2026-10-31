using System;

namespace Inventory.Domain.Common;

// [DDD] Implementación concreta de IDomainEvent.
// Almacena la fecha y nombre del evento para su despacho por el Unit of Work.
public class DomainEvent : IDomainEvent
{
    public DateTime OccurredAt { get; }
    public string EventName { get; }

    public DomainEvent(string eventName)
    {
        EventName = eventName;
        OccurredAt = DateTime.UtcNow;
    }
}