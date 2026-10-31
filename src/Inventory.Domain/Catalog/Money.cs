using System;
using Inventory.Domain.Common;

// Value object que representa un monto de dinero.
public sealed class Money
{
    private readonly decimal _value;
    private const int DecimalPlaces = 2;

    /// <summary>
    /// CAPA: Inventory. Value object Moneda/monto para precios del producto.
    /// POR QUÉ ACÁ: Garantiza que todos los montos del producto sean
    /// consistentes con 2 decimales y nunca negativos, evitando errores
    /// de cálculo en ventas y reportes.
    /// </summary>
    public decimal Value => _value;

    public bool IsZero => _value == 0;
    public bool IsPositive => _value > 0;

    private Money(decimal value)
    {
        _value = value;
    }

    public static Money Create(decimal amount)
    {
        if (amount < 0)
            throw new DomainException("El monto no puede ser negativo.");

        // Redondear a 2 decimales (formato monetario estándar)
        var rounded = Math.Round(amount, DecimalPlaces, MidpointRounding.AwayFromZero);

        if (rounded == 0 && amount != 0)
            throw new DomainException("El monto no puede ser cero a menos que sea exactamente 0.");

        return new Money(rounded);
    }

    public static implicit operator decimal(Money money) => money._value;
    public static implicit operator Money(decimal amount) => Create(amount);

    public static Money operator +(Money a, Money b) => Create(a._value + b._value);
    public static Money operator -(Money a, Money b) => Create(a._value - b._value);
    public static Money operator *(Money a, decimal multiplier) => Create(a._value * multiplier);
    public static bool operator >(Money a, Money b) => a._value > b._value;
    public static bool operator <(Money a, Money b) => a._value < b._value;
    public static bool operator >=(Money a, Money b) => a._value >= b._value;
    public static bool operator <=(Money a, Money b) => a._value <= b._value;

    public override bool Equals(object? obj)
    {
        if (obj is Money other)
            return _value == other._value;
        return false;
    }

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => _value.ToString("F2");
}