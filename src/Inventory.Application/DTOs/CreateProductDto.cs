namespace Inventory.Application.DTOs;

public record CreateProductDto(
    string Plu,
    string Sku,
    string Name,
    string Category,
    decimal Cost,
    decimal Price,
    int MinimumStock,
    int InitialStock);
