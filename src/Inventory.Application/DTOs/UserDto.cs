namespace Inventory.Application.DTOs;

public record UserDto(
    int Id,
    string Username,
    int CompanyId,
    string Role,
    int EmployeeNumber,
    string JobTitle);
