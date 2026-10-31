using System;
using Inventory.Domain.Common;

// Value object que representa el puesto/cargo del usuario.
public sealed class JobTitle
{
    private readonly string _value;

    public string Value => _value;

    private JobTitle(string value) => _value = value ?? throw new DomainException("El puesto no puede ser nulo.");

    public static JobTitle Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("El puesto no puede estar vacío.");

        var normalized = title.Trim();
        return new JobTitle(normalized);
    }

    public static implicit operator string(JobTitle title) => title._value;
    public static implicit operator JobTitle(string title) => Create(title);

    public override bool Equals(object? obj)
    {
        if (obj is JobTitle other)
            return _value == other._value;
        return false;
    }

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => _value;
}