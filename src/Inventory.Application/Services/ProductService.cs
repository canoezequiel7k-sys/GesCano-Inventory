using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Domain.Exceptions;
using Inventory.Domain.Interfaces;

namespace Inventory.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public IEnumerable<ProductDto> GetByCompany(int companyId)
    {
        return _productRepository.GetByCompany(companyId)
            .Select(MapToDto);
    }

    public ProductDto? GetById(int id, int companyId)
    {
        var product = _productRepository.GetById(id);
        if (product is null || product.CompanyId != companyId)
            return null;

        return MapToDto(product);
    }

    public void Create(int companyId, CreateProductDto dto)
    {
        if (_productRepository.GetBySku(dto.Sku, companyId) is not null)
            throw new DuplicateEntityException($"Ya existe un producto con SKU '{dto.Sku}' en esta empresa.");

        var product = new Product
        {
            CompanyId = companyId,
            Sku = dto.Sku,
            Name = dto.Name,
            Category = dto.Category,
            Price = dto.Price,
            Stock = dto.Stock
        };

        _productRepository.Add(product);
    }

    public void Update(int id, int companyId, UpdateProductDto dto)
    {
        var product = _productRepository.GetById(id)
            ?? throw new EntityNotFoundException("Producto", id);

        if (product.CompanyId != companyId)
            throw new InvalidOperationException("No se puede editar un producto de otra empresa.");

        var existing = _productRepository.GetBySku(dto.Sku, companyId);
        if (existing is not null && existing.Id != id)
            throw new DuplicateEntityException($"Ya existe un producto con SKU '{dto.Sku}' en esta empresa.");

        product.Sku = dto.Sku;
        product.Name = dto.Name;
        product.Category = dto.Category;
        product.Price = dto.Price;
        product.Stock = dto.Stock;

        _productRepository.Update(product);
    }

    public void Delete(int id, int companyId)
    {
        var product = _productRepository.GetById(id)
            ?? throw new EntityNotFoundException("Producto", id);

        if (product.CompanyId != companyId)
            throw new InvalidOperationException("No se puede eliminar un producto de otra empresa.");

        _productRepository.Delete(id);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto(
            product.Id,
            product.Sku,
            product.Name,
            product.Category,
            product.Price,
            product.Stock);
    }
}
