using System;

namespace Inventory.Domain.Common;

// [DDD] Entidad base para todas las entidades de dominio.
// Vive en Domain porque es lógica pura, sin dependencias de EF ni tecnología.
// Proporciona Id y métodos comunes como Equals/GetHashCode basados en Id.
public abstract class Entity
{
    public int Id { get; protected set; }
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected Entity() { }

    protected Entity(int id) => Id = id;

    public override bool Equals(object? obj)
    {
        if (obj is Entity other)
        {
            return Id == other.Id;
        }
        return false;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString() => $"{GetType().Name} [{Id}]";
}