using System;
using Inventory.Domain.Common;
using Inventory.Domain.Catalog;
using Inventory.Domain.Inventory;

// [DDD] Value object inmutable que registra un movimiento de stock.
// Cada movimiento es un registro de entrada/salida con propósito, usuario y fecha.
// No tiene métodos para modificar su estado - si se necesita una corrección,
// se registra un nuevo movimiento de ajuste.
public sealed class StockMovement : Entity
{
    // Datos del movimiento
    public int CompanyId { get; private set; }
    public int ProductId { get; private set; }
    public MovementType Type { get; private set; } // Receipt, Sale, Adjustment
    public Quantity Quantity { get; private set; } = null!; // Cantidad del movimiento (las ventas restan en StockItem)
    public DateTime OccurredAt { get; private set; }
    public int UserId { get; private set; } // Quien realizó el movimiento
    public string? Reference { get; private set; } // ID de venta, remito, etc.
    public string? Reason { get; private set; } // Motivo obligatorio en ajustes

    // Factory methods

    public static StockMovement Receipt(int companyId, int productId, int userId, Quantity quantity, string? reference = null)
    {
        if (quantity.IsNegative)
            throw new DomainException("La cantidad de una recepción debe ser positiva.");

        var movement = new StockMovement
        {
            Id = 0, // Se asigna por EF al persistir
            CompanyId = companyId,
            ProductId = productId,
            Type = MovementType.Receipt,
            Quantity = quantity,
            OccurredAt = DateTime.UtcNow,
            UserId = userId,
            Reference = reference,
            Reason = null
        };

        movement.AddDomainEvent(new DomainEvent("StockReceived"));
        return movement;
    }

    public static StockMovement Sale(int companyId, int productId, int userId, Quantity quantity, string? reference = null, string? reason = null)
    {
        var movement = new StockMovement
        {
            Id = 0,
            CompanyId = companyId,
            ProductId = productId,
            Type = MovementType.Sale,
            Quantity = quantity, // Ventas registran la cantidad vendida (el signo lo interpreta el StockItem)
            OccurredAt = DateTime.UtcNow,
            UserId = userId,
            Reference = reference,
            Reason = reason
        };

        movement.AddDomainEvent(new DomainEvent("StockSold"));
        return movement;
    }

    public static StockMovement Adjustment(int companyId, int productId, int userId, Quantity quantity, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("El motivo es obligatorio para un ajuste de stock.");

        if (quantity.IsZero)
            throw new DomainException("La cantidad de un ajuste no puede ser cero.");

        var movement = new StockMovement
        {
            Id = 0,
            CompanyId = companyId,
            ProductId = productId,
            Type = MovementType.Adjustment,
            Quantity = quantity,
            OccurredAt = DateTime.UtcNow,
            UserId = userId,
            Reason = reason
        };

        movement.AddDomainEvent(new DomainEvent("StockAdjusted"));
        return movement;
    }

    // Obtiene la cantidad con el signo apropiado según el tipo
    public Quantity GetSignedQuantity()
    {
        return Quantity;
    }

    internal void SetCompanyId(int companyId) => CompanyId = companyId;
    internal void SetProductId(int productId) => ProductId = productId;
    internal void SetUserId(int userId) => UserId = userId;
}