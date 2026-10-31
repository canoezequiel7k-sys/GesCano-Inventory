namespace Inventory.Application.DTOs;

public record ProductDto(
    int Id,
    string Sku,
    string Name,
    string Category,
    decimal Price,
    int Stock);
