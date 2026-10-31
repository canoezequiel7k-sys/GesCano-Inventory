using System;
using Inventory.Domain.Common;
using Inventory.Domain.Catalog;
using Inventory.Domain.Inventory;

// [DDD] Agregado raíz del bounded context Catalog.
// Representa un producto que la empresa compra y/o vende.
// Tiene invariantes: PLU y SKU únicos por empresa, descripción obligatoria,
// precio y costo ≥ 0.
public sealed class Product : Entity
{
    // Data de la empresa
    public int CompanyId { get; private set; }

    // Value objects (inmutables)
    public Plu Plu { get; private set; }
    public Sku Sku { get; private set; }

    // Descripción y categoría
    public string Description { get; private set; }
    public string Category { get; private set; }

    // Precios (value object Money)
    public Money UnitCost { get; private set; }
    public Money UnitPrice { get; private set; }

    // Configuración de stock
    public int MinimumStock { get; private set; } // Stock mínimo para alertas

    // Referencia al stock item (agregado separada pero relacionado)
    public int? StockItemId { get; private set; }

    // Navegación (no mapeadas directamente en EF a menos que sean FK)
    // public StockItem Stock { get; private set; } ← sería navigation property

    // Constructor factory - encapsula la creación con validaciones
    public static Product Create(int companyId, Plu plu, Sku sku, string description,
        string category, Money unitCost, Money unitPrice, int minimumStock)
    {
        // Invariantes de negocio
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("La descripción del producto es obligatoria.");

        if (string.IsNullOrWhiteSpace(category))
            throw new DomainException("La categoría del producto es obligatoria.");

        if (unitCost.Value < 0)
            throw new DomainException("El costo unitario no puede ser negativo.");

        if (unitPrice.Value < 0)
            throw new DomainException("El precio unitario no puede ser negativo.");

        if (unitPrice.Value < unitCost.Value)
            throw new DomainException("El precio de venta debe ser mayor o igual al costo.");

        if (minimumStock < 0)
            throw new DomainException("El stock mínimo no puede ser negativo.");

        var product = new Product
        {
            Id = 0,
            CompanyId = companyId,
            Plu = plu,
            Sku = sku,
            Description = description,
            Category = category,
            UnitCost = unitCost,
            UnitPrice = unitPrice,
            MinimumStock = minimumStock,
            StockItemId = null // Se crea separado
        };

        product.AddDomainEvent(new DomainEvent("ProductCreated"));
        return product;
    }

    // Métodos de actualización (solo cambiar lo necesario)

    public void UpdateDescription(string newDescription)
    {
        if (string.IsNullOrWhiteSpace(newDescription))
            throw new DomainException("La descripción no puede ser vacía.");

        Description = newDescription;
        AddDomainEvent(new DomainEvent("ProductDescriptionUpdated"));
    }

    public void UpdateCategory(string newCategory)
    {
        if (string.IsNullOrWhiteSpace(newCategory))
            throw new DomainException("La categoría no puede ser vacía.");

        Category = newCategory;
        AddDomainEvent(new DomainEvent("ProductCategoryUpdated"));
    }

    public void UpdatePrice(Money newCost, Money newPrice)
    {
        if (newCost.Value < 0)
            throw new DomainException("El costo no puede ser negativo.");

        if (newPrice.Value < 0)
            throw new DomainException("El precio no puede ser negativo.");

        if (newPrice.Value < newCost.Value)
            throw new DomainException("El precio de venta debe ser mayor o igual al costo.");

        UnitCost = newCost;
        UnitPrice = newPrice;
        AddDomainEvent(new DomainEvent("ProductPriceUpdated"));
    }

    public void UpdateMinimumStock(int newMinimum)
    {
        if (newMinimum < 0)
            throw new DomainException("El stock mínimo no puede ser negativo.");

        MinimumStock = newMinimum;
        AddDomainEvent(new DomainEvent("ProductMinimumStockUpdated"));
    }

    // Operadores de comparación
    public bool SamePluAs(Product other) => Plu.Value == other.Plu.Value;
    public bool SameSkuAs(Product other) => Sku.Value == other.Sku.Value;
    public bool SameCompanyAs(Product other) => CompanyId == other.CompanyId;

    public override bool Equals(object? obj)
    {
        if (obj is Product other)
            return Id == other.Id && CompanyId == other.CompanyId;
        return false;
    }

    public override int GetHashCode() => (Id, CompanyId).GetHashCode();

    public override string ToString() => $"Product [Id={Id}, SKU={Sku}, PLU={Plu}, Description={Description}]";
}