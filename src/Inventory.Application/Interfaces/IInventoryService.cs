using Inventory.Application.DTOs;

namespace Inventory.Application.Interfaces;

public interface IInventoryService
{
    void CreateProduct(CreateProductDto dto);
    ProductDto? GetProduct(string sku);
    IEnumerable<ProductDto> GetAllProducts();
    InventorySummaryDto GetSummary();
    void AdjustStock(string sku, int quantityChange);
}
