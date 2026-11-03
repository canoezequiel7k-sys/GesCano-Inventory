using Inventory.Domain.Catalog;
using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public Product? GetById(int id) => _context.Products.Find(id);

    public Product? GetBySku(string sku, int companyId)
    {
        var normalized = Sku.Create(sku).Value;

        return _context.Products
            .Where(p => p.CompanyId == companyId)
            .ToList()
            .FirstOrDefault(p => p.Sku.Value == normalized);
    }

    public Product? GetByPlu(string plu, int companyId)
    {
        var normalized = Plu.Create(plu).Value;

        return _context.Products
            .Where(p => p.CompanyId == companyId)
            .ToList()
            .FirstOrDefault(p => p.Plu.Value == normalized);
    }

    public IEnumerable<Product> GetByCompany(int companyId) =>
        _context.Products
            .Where(p => p.CompanyId == companyId)
            .ToList()
            .OrderBy(p => p.Description)
            .ToList();

    public void Add(Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var entity = _context.Products.Find(id);
        if (entity is not null)
        {
            _context.Products.Remove(entity);
            _context.SaveChanges();
        }
    }
}
