using Inventory.Application.DTOs;

namespace Inventory.Application.Interfaces;

public interface ICompanyService
{
    CompanyDto? GetById(int id);
    IEnumerable<CompanyDto> GetAll();
}
