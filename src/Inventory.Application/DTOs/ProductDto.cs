namespace Inventory.Application.DTOs;

public record ProductDto(
    int Id,
    string Plu,
    string Sku,
    string Name,
    string Category,
    decimal Cost,
    decimal Price,
    int MinimumStock,
    int Stock,
    string? ImagePath = null)
{
    public bool IsLowStock => Stock <= MinimumStock;
    public string Status => IsLowStock ? "Stock bajo" : "Normal";
    public decimal InventoryValue => Cost * Stock;
}
