using System;
using Inventory.Domain.Common;

namespace Inventory.Domain.Inventory;

// Value object que representa una cantidad de producto en stock.
// Es inmutable y se valida al crearse.
// Soporta tanto cantidades enteras (productos unitarios) como decimales
// (productos por peso como frutas, verduras, etc.).
public sealed class Quantity
{
    private readonly decimal _value;
    private readonly bool _allowFractions;

    public decimal Value => _value;

    public bool IsZero => _value == 0;
    public bool IsPositive => _value > 0;
    public bool IsNegative => _value < 0;

    private Quantity(decimal value, bool allowFractions)
    {
        _value = value;
        _allowFractions = allowFractions;
    }

    public static Quantity Create(decimal value, bool allowFractions = false)
    {
        if (value < 0)
            throw new DomainException("La cantidad no puede ser negativa.");

        if (!allowFractions && value % 1m != 0)
            throw new DomainException("Las cantidades fraccionarias no están permitidas en este contexto.");

        return new Quantity(value, allowFractions);
    }

    public static Quantity CreateWhole(decimal value) => Create(value, allowFractions: false);
    public static Quantity CreateFractional(decimal value) => Create(value, allowFractions: true);

    public static implicit operator decimal(Quantity quantity) => quantity._value;
    public static implicit operator Quantity(decimal value) => Create(value);

    public static bool operator >(Quantity a, Quantity b) => a._value > b._value;
    public static bool operator <(Quantity a, Quantity b) => a._value < b._value;
    public static bool operator >=(Quantity a, Quantity b) => a._value >= b._value;
    public static bool operator <=(Quantity a, Quantity b) => a._value <= b._value;

    public Quantity Add(Quantity other) => Create(_value + other._value);
    public Quantity Subtract(Quantity other) => Create(_value - other._value);

    public override bool Equals(object? obj)
    {
        if (obj is Quantity other)
            return _value == other._value && _allowFractions == other._allowFractions;
        return false;
    }

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => _value.ToString("F2");
}