namespace Inventory.Application.DTOs;

public record UpdateProductDto(
    string Name,
    string Category,
    decimal Cost,
    decimal Price,
    int MinimumStock);
