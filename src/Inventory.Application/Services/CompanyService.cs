using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Domain.Exceptions;
using Inventory.Domain.Interfaces;

namespace Inventory.Application.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyService(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public CompanyDto? GetById(int id)
    {
        var company = _companyRepository.GetById(id);
        return company is null ? null : new CompanyDto(company.Id, company.Name);
    }

    public IEnumerable<CompanyDto> GetAll()
    {
        return _companyRepository.GetAll()
            .Select(c => new CompanyDto(c.Id, c.Name));
    }
}
