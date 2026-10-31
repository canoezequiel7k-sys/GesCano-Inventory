namespace Inventory.Application.DTOs;

public record CreateProductDto(
    string Sku,
    string Name,
    string Category,
    decimal Price,
    int Stock);
