using System;

namespace Inventory.Domain.Common;

// [DDD] Interfaz para eventos de dominio.
// Se despacha dentro de la transacción del Unit of Work.
// Permite notificar a otros partes del sistema sobre cambios importantes
// sin acoplar el dominio a suscripciones concretas.
public interface IDomainEvent
{
    DateTime OccurredAt { get; }
    string EventName { get; }
}