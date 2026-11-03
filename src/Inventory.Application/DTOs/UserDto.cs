namespace Inventory.Application.DTOs;

public record UserDto(
    int Id,
    string Username,
    int CompanyId,
    string Role,
    int EmployeeNumber,
    string JobTitle)
{
    // Solo gerencia y jefatura de salón pueden crear, actualizar o eliminar productos.
    public bool CanManageCatalog => Role is "Admin" or "Manager" or "FloorManager";
}
