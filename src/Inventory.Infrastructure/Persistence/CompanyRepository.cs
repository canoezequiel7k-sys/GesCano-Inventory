using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence;

public class CompanyRepository : ICompanyRepository
{
    private readonly AppDbContext _context;

    public CompanyRepository(AppDbContext context)
    {
        _context = context;
    }

    public Company? GetById(int id) => _context.Companies.Find(id);

    public IEnumerable<Company> GetAll() => _context.Companies.OrderBy(c => c.Name).ToList();

    public void Add(Company company)
    {
        _context.Companies.Add(company);
        _context.SaveChanges();
    }

    public void Update(Company company)
    {
        _context.Companies.Update(company);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var entity = _context.Companies.Find(id);
        if (entity is not null)
        {
            _context.Companies.Remove(entity);
            _context.SaveChanges();
        }
    }
}
