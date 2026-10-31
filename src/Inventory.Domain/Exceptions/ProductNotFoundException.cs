namespace Inventory.Domain.Exceptions;

public class ProductNotFoundException : Exception
{
    public ProductNotFoundException(string sku)
        : base($"Producto con SKU '{sku}' no encontrado en el inventario.")
    {
    }
}

public class DuplicateProductException : Exception
{
    public DuplicateProductException(string sku)
        : base($"Ya existe un producto con SKU '{sku}' en el inventario.")
    {
    }
}
