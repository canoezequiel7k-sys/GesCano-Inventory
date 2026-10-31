using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public User? GetByUsername(string username) =>
        _context.Users.Include(u => u.Company).FirstOrDefault(u => u.Username == username);

    public User? GetById(int id) => _context.Users.Find(id);

    public IEnumerable<User> GetByCompany(int companyId) =>
        _context.Users.Where(u => u.CompanyId == companyId).OrderBy(u => u.Username).ToList();

    public void Add(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var entity = _context.Users.Find(id);
        if (entity is not null)
        {
            _context.Users.Remove(entity);
            _context.SaveChanges();
        }
    }
}
