using Inventory.Application.DTOs;

namespace Inventory.Application.Interfaces;

public interface IProductService
{
    IEnumerable<ProductDto> GetByCompany(int companyId);
    ProductDto? GetById(int id, int companyId);
    void Create(int companyId, CreateProductDto dto);
    void Update(int id, int companyId, UpdateProductDto dto);
    void Delete(int id, int companyId);
}
