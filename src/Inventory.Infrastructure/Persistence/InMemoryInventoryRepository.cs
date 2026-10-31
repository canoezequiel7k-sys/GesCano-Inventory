using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Domain.Exceptions;

namespace Inventory.Infrastructure.Persistence;

public class InMemoryInventoryRepository : IInventoryRepository
{
    private readonly Dictionary<string, Product> _products = new(StringComparer.OrdinalIgnoreCase);

    public void Add(Product product)
    {
        if (_products.ContainsKey(product.Sku))
            throw new DuplicateProductException(product.Sku);

        _products[product.Sku] = product;
    }

    public Product GetBySku(string sku)
    {
        return _products.TryGetValue(sku, out var product)
            ? product
            : throw new ProductNotFoundException(sku);
    }

    public IEnumerable<Product> GetAll()
    {
        return _products.Values.OrderBy(p => p.Sku).ToList();
    }

    public void Update(Product product)
    {
        if (!_products.ContainsKey(product.Sku))
            throw new ProductNotFoundException(product.Sku);

        _products[product.Sku] = product;
    }

    public bool Exists(string sku)
    {
        return _products.ContainsKey(sku);
    }
}
