using Inventory.Domain.Entities;

namespace Inventory.Application.Interfaces;

public interface IInventoryRepository
{
    void Add(Product product);
    Product GetBySku(string sku);
    IEnumerable<Product> GetAll();
    void Update(Product product);
    bool Exists(string sku);
}
