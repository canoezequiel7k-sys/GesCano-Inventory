using System;

namespace Inventory.Domain.Identity;

// [NEGOCIO] El orden importa para la comparación HasRole():
// Admin > Manager > FloorManager > Employee.
public enum UserRole
{
    Employee = 1,      // Repositor / usuario estándar (solo consulta)
    FloorManager = 2,  // Jefe de salón
    Manager = 3,       // Gerente
    Admin = 4          // Administrador con todos los permisos
}
