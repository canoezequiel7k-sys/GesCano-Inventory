using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.Domain.Catalog;
using Inventory.Domain.Common;
using Inventory.Domain.Entities;
using Inventory.Domain.Exceptions;
using Inventory.Domain.Interfaces;
using Inventory.Domain.Inventory;

namespace Inventory.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IStockItemRepository _stockRepository;

    public ProductService(
        IProductRepository productRepository,
        IStockItemRepository stockRepository)
    {
        _productRepository = productRepository;
        _stockRepository = stockRepository;
    }

    public IEnumerable<ProductDto> GetByCompany(int companyId)
    {
        var stocks = _stockRepository.GetByCompany(companyId)
            .ToDictionary(s => s.ProductId, s => s.CurrentStock.Value);

        return _productRepository.GetByCompany(companyId)
            .Select(product => MapToDto(product, stocks.GetValueOrDefault(product.Id)));
    }

    public ProductDto? GetById(int id, int companyId)
    {
        var product = _productRepository.GetById(id);
        if (product is null || product.CompanyId != companyId)
            return null;

        var stock = _stockRepository.GetByProduct(id)?.CurrentStock.Value ?? 0m;
        return MapToDto(product, stock);
    }

    public ProductDto? GetByCode(string code, int companyId)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        var trimmed = code.Trim();
        var product = _productRepository.GetBySku(trimmed, companyId)
            ?? _productRepository.GetByPlu(trimmed, companyId);

        if (product is null)
            return null;

        var stock = _stockRepository.GetByProduct(product.Id)?.CurrentStock.Value ?? 0m;
        return MapToDto(product, stock);
    }

    public void Create(int companyId, CreateProductDto dto)
    {
        if (_productRepository.GetBySku(dto.Sku, companyId) is not null)
            throw new DuplicateEntityException($"Ya existe un producto con SKU '{dto.Sku}' en esta empresa.");

        if (_productRepository.GetByPlu(dto.Plu, companyId) is not null)
            throw new DuplicateEntityException($"Ya existe un producto con PLU '{dto.Plu}' en esta empresa.");

        if (dto.InitialStock < 0)
            throw new DomainException("El stock inicial no puede ser negativo.");

        var product = Product.Create(
            companyId,
            Plu.Create(dto.Plu),
            Sku.Create(dto.Sku),
            dto.Name,
            dto.Category,
            Money.Create(dto.Cost),
            Money.Create(dto.Price),
            dto.MinimumStock,
            dto.ImagePath);

        _productRepository.Add(product);

        var stockItem = StockItem.Create(product.Id, companyId);

        if (dto.InitialStock > 0)
        {
            var movement = StockMovement.Receipt(
                companyId,
                product.Id,
                userId: 0,
                Quantity.CreateWhole(dto.InitialStock),
                reference: "Carga inicial");

            stockItem.AddMovement(movement);
            _stockRepository.Add(stockItem);
            _stockRepository.AddMovement(movement);
        }
        else
        {
            _stockRepository.Add(stockItem);
        }

        product.LinkStockItem(stockItem.Id);
        _productRepository.Update(product);
    }

    public void Update(int id, int companyId, UpdateProductDto dto)
    {
        var product = _productRepository.GetById(id)
            ?? throw new EntityNotFoundException("Producto", id);

        if (product.CompanyId != companyId)
            throw new InvalidOperationException("No se puede editar un producto de otra empresa.");

        product.UpdateDescription(dto.Name);
        product.UpdateCategory(dto.Category);
        product.UpdatePrice(Money.Create(dto.Cost), Money.Create(dto.Price));
        product.UpdateMinimumStock(dto.MinimumStock);

        _productRepository.Update(product);
    }

    public void Delete(int id, int companyId)
    {
        var product = _productRepository.GetById(id)
            ?? throw new EntityNotFoundException("Producto", id);

        if (product.CompanyId != companyId)
            throw new InvalidOperationException("No se puede eliminar un producto de otra empresa.");

        _stockRepository.DeleteByProduct(id);
        _productRepository.Delete(id);
    }

    private static ProductDto MapToDto(Product product, decimal stock)
    {
        return new ProductDto(
            product.Id,
            product.Plu.Value,
            product.Sku.Value,
            product.Description,
            product.Category,
            product.UnitCost.Value,
            product.UnitPrice.Value,
            product.MinimumStock,
            (int)Math.Floor(stock),
            product.ImagePath);
    }
}
