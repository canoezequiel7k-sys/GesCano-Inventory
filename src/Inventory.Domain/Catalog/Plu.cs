using System;
using System.Text.RegularExpressions;
using Inventory.Domain.Common;

namespace Inventory.Domain.Catalog;

// [DDD] Value object que representa un Código PLU (Price Look-Up).
// Se usa para la venta rápida de productos (tipo balanza/caja).
// Validaciones:
// - No vacío después del trim
// - Solo dígitos
// - Longitud entre 1 y 8 caracteres
public sealed class Plu
{
    private readonly string _value;
    private const int MinLength = 1;
    private const int MaxLength = 8;
    private static readonly Regex _digitOnly = new(@"^\d+$", RegexOptions.Compiled);

    /// <summary>
    /// CAPA: Domain. Value object PLU para identificación rápida de producto en ventas.
    /// POR QUÉ ACÁ: Es una regla de negocio que el PLU debe ser solo dígitos
    /// de 1 a 8 caracteres, usado en puntos de venta por teclado numérico.
    /// </summary>
    public string Value => _value;

    public int Length => _value.Length;

    private Plu(string value)
    {
        _value = value;
    }

    public static Plu Create(string? plu)
    {
        if (string.IsNullOrWhiteSpace(plu))
            throw new DomainException("El PLU no puede ser nulo o vacío.");

        var trimmed = plu.Trim();

        if (trimmed.Length < MinLength || trimmed.Length > MaxLength)
            throw new DomainException($"El PLU debe tener entre {MinLength} y {MaxLength} dígitos.");

        if (!_digitOnly.IsMatch(trimmed))
            throw new DomainException("El PLU debe contener solo dígitos numéricos.");

        return new Plu(trimmed);
    }

    public static implicit operator string(Plu plu) => plu._value;
    public static implicit operator Plu(string plu) => Create(plu);

    public override bool Equals(object? obj)
    {
        if (obj is Plu other)
            return _value == other._value;
        return false;
    }

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => _value;
}
