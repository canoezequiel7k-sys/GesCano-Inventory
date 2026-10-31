using Inventory.Domain.Entities;

namespace Inventory.Domain.Interfaces;

public interface ICompanyRepository
{
    Company? GetById(int id);
    IEnumerable<Company> GetAll();
    void Add(Company company);
    void Update(Company company);
    void Delete(int id);
}
