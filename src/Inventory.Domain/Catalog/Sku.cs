using System;
using System.Text.RegularExpressions;
using Inventory.Domain.Common;

// [DDD] Value object que representa un SKU (Stock Keeping Unit).
// Código interno de inventario, único por empresa.
// Validaciones:
// - No vacío después del trim
// - Alfanumérico con guiones permitidos
// - Máximo 32 caracteres
// - Se normaliza a mayúsculas
public sealed class Sku
{
    private readonly string _value;
    private const int MaxLength = 32;
    private static readonly Regex _skuPattern = new(@"^[A-Za-z0-9_-]+$", RegexOptions.Compiled);

    /// <summary>
    /// CAPA: Domain. Value object SKU para identificación interna de producto.
    /// POR QUÉ ACÁ: El SKU es el código de inventario que identifica al producto
    /// dentro de la empresa. Permite guiones y debe ser alfanumérico, máximo 32 chars.
    /// </summary>
    public string Value => _value;

    public int Length => _value.Length;

    private Sku(string value)
    {
        _value = value;
    }

    public static Sku Create(string? sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("El SKU no puede ser nulo o vacío.");

        var trimmed = sku.Trim();

        if (trimmed.Length > MaxLength)
            throw new DomainException($"El SKU no puede exceder {MaxLength} caracteres.");

        if (!_skuPattern.IsMatch(trimmed))
            throw new DomainException("El SKU debe ser alfanumérico con guiones permitidos (ej.: ART-001).");

        // Normalizar a mayúsculas para unicidad consistente
        var normalized = trimmed.ToUpperInvariant();

        return new Sku(normalized);
    }

    public static implicit operator string(Sku sku) => sku._value;
    public static implicit operator Sku(string sku) => Create(sku);

    public override bool Equals(object? obj)
    {
        if (obj is Sku other)
            return _value == other._value;
        return false;
    }

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => _value;
}