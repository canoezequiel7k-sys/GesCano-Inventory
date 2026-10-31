using Inventory.Domain.Entities;

namespace Inventory.Domain.Interfaces;

public interface IUserRepository
{
    User? GetByUsername(string username);
    User? GetById(int id);
    IEnumerable<User> GetByCompany(int companyId);
    void Add(User user);
    void Update(User user);
    void Delete(int id);
}
