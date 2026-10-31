using System;
using Inventory.Domain.Common;

// Value object que representa el número de legajo de un empleado.
public sealed class EmployeeNumber
{
    private readonly int _value;

    public int Value => _value;

    private EmployeeNumber(int value) => _value = value;

    public static EmployeeNumber Create(int employeeNumber)
    {
        if (employeeNumber <= 0)
            throw new DomainException("El legajo debe ser un entero positivo.");

        return new EmployeeNumber(employeeNumber);
    }

    public static implicit operator int(EmployeeNumber empNum) => empNum._value;
    public static implicit operator EmployeeNumber(int empNum) => Create(empNum);

    public override bool Equals(object? obj)
    {
        if (obj is EmployeeNumber other)
            return _value == other._value;
        return false;
    }

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => _value.ToString();
}