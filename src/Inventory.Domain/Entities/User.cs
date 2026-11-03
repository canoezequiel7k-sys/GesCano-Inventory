using System;
using Inventory.Domain.Common;
using Inventory.Domain.Identity;

namespace Inventory.Domain.Entities;

// [DDD] Agregado raíz del bounded context Identity.
// Representa un usuario del sistema, perteneciente a una empresa.
// Invariantes: Legajo único por empresa, hash de contraseña obligatorio,
// rol dentro de UserRole (Admin, Employee).
public sealed class User : Entity
{
    // Datos de la empresa
    public int CompanyId { get; private set; }

    // Navegación usada por EF Core (no se modifica desde la UI)
    public Company? Company { get; private set; }

    // Identificación personal
    public EmployeeNumber EmployeeNumber { get; private set; } = null!;
    public JobTitle JobTitle { get; private set; } = null!;

    // Autenticación
    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    // Rol y estado
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }

    // Constructor vacío requerido por EF Core.
    private User() { }

    // Constructor factory
    public static User Create(int companyId, EmployeeNumber employeeNumber, JobTitle jobTitle,
        string username, string passwordHash, UserRole role, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new DomainException("El nombre de usuario es obligatorio.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("El hash de contraseña es obligatorio.");

        var user = new User
        {
            Id = 0,
            CompanyId = companyId,
            EmployeeNumber = employeeNumber,
            JobTitle = jobTitle,
            Username = username.Trim(),
            PasswordHash = passwordHash,
            Role = role,
            IsActive = isActive
        };

        user.AddDomainEvent(new DomainEvent("UserCreated"));
        return user;
    }

    // Métodos de actualización

    public void UpdatePasswordHash(string newHash)
    {
        if (string.IsNullOrWhiteSpace(newHash))
            throw new DomainException("El hash de contraseña no puede estar vacío.");

        PasswordHash = newHash;
        AddDomainEvent(new DomainEvent("UserPasswordUpdated"));
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    public void UpdateJobTitle(JobTitle newTitle) => JobTitle = newTitle;

    public bool HasRole(UserRole requiredRole) => Role >= requiredRole; // Admin >= Employee

    public override bool Equals(object? obj)
    {
        if (obj is User other)
            return Id == other.Id && CompanyId == other.CompanyId;
        return false;
    }

    public override int GetHashCode() => (Id, CompanyId).GetHashCode();

    public override string ToString() => $"User [Username={Username}, EmployeeNumber={EmployeeNumber}, Role={Role}]";
}
