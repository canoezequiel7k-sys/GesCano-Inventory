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

    public Product? GetBySku(string sku, int companyId) =>
        _context.Products.FirstOrDefault(p => p.Sku == sku && p.CompanyId == companyId);

    public IEnumerable<Product> GetByCompany(int companyId) =>
        _context.Products.Where(p => p.CompanyId == companyId)
            .OrderBy(p => p.Sku)
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
