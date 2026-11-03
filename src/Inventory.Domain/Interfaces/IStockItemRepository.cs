using Inventory.Domain.Inventory;

namespace Inventory.Domain.Interfaces;

public interface IStockItemRepository
{
    StockItem? GetByProduct(int productId);
    IEnumerable<StockItem> GetByCompany(int companyId);
    void Add(StockItem stockItem);
    void Update(StockItem stockItem);
    void AddMovement(StockMovement movement);
    void DeleteByProduct(int productId);
}
