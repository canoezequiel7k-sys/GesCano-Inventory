using System;
using Inventory.Domain.Common;

// [DDD] Enumeración de tipos de movimiento de stock.
// Se usa para distinguir recepciones, ventas y ajustes en el StockItem.
public enum MovementType
{
    Receipt, // Entrada de mercadería (recepción)
    Sale,    // Salida por venta
    Adjustment // Corrección de stock
}