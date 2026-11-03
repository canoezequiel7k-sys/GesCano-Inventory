using Inventory.Domain.Interfaces;
using Inventory.Domain.Inventory;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence;

public class StockItemRepository : IStockItemRepository
{
    private readonly AppDbContext _context;

    public StockItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public StockItem? GetByProduct(int productId) =>
        _context.StockItems.FirstOrDefault(s => s.ProductId == productId);

    public IEnumerable<StockItem> GetByCompany(int companyId) =>
        _context.StockItems.Where(s => s.CompanyId == companyId).ToList();

    public void Add(StockItem stockItem)
    {
        _context.StockItems.Add(stockItem);
        _context.SaveChanges();
    }

    public void Update(StockItem stockItem)
    {
        _context.StockItems.Update(stockItem);
        _context.SaveChanges();
    }

    public void AddMovement(StockMovement movement)
    {
        _context.StockMovements.Add(movement);
        _context.SaveChanges();
    }

    public void DeleteByProduct(int productId)
    {
        var movements = _context.StockMovements
            .Where(m => m.ProductId == productId)
            .ToList();

        if (movements.Count > 0)
            _context.StockMovements.RemoveRange(movements);

        var stockItem = _context.StockItems.FirstOrDefault(s => s.ProductId == productId);
        if (stockItem is not null)
            _context.StockItems.Remove(stockItem);

        _context.SaveChanges();
    }
}
