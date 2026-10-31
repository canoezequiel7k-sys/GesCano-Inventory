namespace Inventory.Application.DTOs;

public record InventorySummaryDto(
    int TotalProducts,
    int TotalUnits,
    decimal TotalValue,
    IEnumerable<CategorySummaryDto> Categories);

public record CategorySummaryDto(string Category, int ProductCount, int Units, decimal Value);
