namespace Inventory.Application.DTOs;

public record UpdateProductDto(
    string Sku,
    string Name,
    string Category,
    decimal Price,
    int Stock);
