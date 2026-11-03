using System;
using Inventory.Domain.Common;
using Inventory.Domain.Catalog;
using Inventory.Domain.Inventory;

// [DDD] Agregado raíz que gestiona el stock de un producto en una empresa.
// Responsabilidades:
// 1. Garantizar que el saldo nunca sea negativo
// 2. Registrar cada cambio de stock como un StockMovement immutable
// 3. Mantener la invariante: saldo = suma de todos los movimientos
// 4. Controlar concurrencia optimista con versión de fila

public sealed class StockItem
{
    // Identidad
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public int CompanyId { get; private set; }
    public int ConcurrencyToken { get; private set; } // Token optimista para concurrencia

    // Estado
    public Quantity CurrentStock { get; private set; } = null!;
    private readonly List<StockMovement> _movements = new(); // Histórico de movimientos

    // Eventos de dominio
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<StockMovement> Movements => _movements.AsReadOnly();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Constructor privado - solo factory methods
    private StockItem() { }

    // Factory method para crear un nuevo StockItem (al crear el producto)
    public static StockItem Create(int productId, int companyId)
    {
        var stockItem = new StockItem
        {
            Id = 0,
            ProductId = productId,
            CompanyId = companyId,
            CurrentStock = Quantity.CreateWhole(0),
            ConcurrencyToken = 1 // Empieza en 1
        };

        stockItem.AddDomainEvent(new DomainEvent("StockItemCreated"));
        return stockItem;
    }

    // Método para agregar un movimiento (llamado dentro de Unit of Work)
    public void AddMovement(StockMovement movement)
    {
        // [NEGOCIO] Las ventas restan stock aunque la cantidad se registre en positivo.
        var signedQuantity = movement.Type == MovementType.Sale
            ? -movement.Quantity.Value
            : movement.Quantity.Value;

        // Validación de invariante: saldo nunca puede quedar negativo después del movimiento
        if (CurrentStock.Value + signedQuantity < 0)
            throw new DomainException($"No se puede realizar este movimiento. Stock actual: {CurrentStock}, movimiento: {signedQuantity} resultaría en stock negativo.");

        _movements.Add(movement);
        CurrentStock = Quantity.Create(CurrentStock.Value + signedQuantity);
        ConcurrencyToken++;

        // Agregar evento según tipo
        string eventName = movement.Type switch
        {
            MovementType.Receipt => "StockReceived",
            MovementType.Sale => "StockSold",
            MovementType.Adjustment => "StockAdjusted",
            _ => "StockModified"
        };
        AddDomainEvent(new DomainEvent(eventName));
    }

    // Método para obtener el saldo teórico sumando movimientos (para validación)
    public decimal GetTheoreticalStock()
    {
        decimal total = 0;
        foreach (var movement in _movements)
        {
            total += movement.Quantity.Value;
        }
        return total;
    }

    private void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    // Limpiar eventos después de que el Unit of Work los procese
    internal void ClearEvents() => _domainEvents.Clear();

    internal void SetConcurrencyToken(int token) => ConcurrencyToken = token;

    public override string ToString() => $"StockItem [ProductId={ProductId}, CompanyId={CompanyId}, Stock={CurrentStock}, Token={ConcurrencyToken}]";
}