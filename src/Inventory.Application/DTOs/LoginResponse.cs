namespace Inventory.Application.DTOs;

public record LoginResponse(
    int UserId,
    string Username,
    int CompanyId,
    string CompanyName,
    string Role,
    int EmployeeNumber,
    string JobTitle);
