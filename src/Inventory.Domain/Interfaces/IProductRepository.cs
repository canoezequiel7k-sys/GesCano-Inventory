using Inventory.Domain.Entities;

namespace Inventory.Domain.Interfaces;

public interface IProductRepository
{
    Product? GetById(int id);
    Product? GetBySku(string sku, int companyId);
    Product? GetByPlu(string plu, int companyId);
    IEnumerable<Product> GetByCompany(int companyId);
    void Add(Product product);
    void Update(Product product);
    void Delete(int id);
}
