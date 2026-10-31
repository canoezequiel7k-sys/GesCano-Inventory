using System;

namespace Inventory.Domain.Identity;

// [Obsolet("Use the comparison methods of User instead", false)] // Comentado temporalmente pending compilation
public enum UserRole
{
    // [NEGOCIO] El orden importa para la comparación HasRole(): Admin > Employee
    Employee = 1,      // Usuario estándar del sistema
    Admin = 2          // Administrador con todos los permisos
}