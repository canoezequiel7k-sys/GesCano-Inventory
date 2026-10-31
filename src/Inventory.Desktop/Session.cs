using Inventory.Application.DTOs;

namespace Inventory.Desktop;

public static class Session
{
    public static UserDto? User { get; set; }
    public static CompanyDto? Company { get; set; }
    public static bool IsLoggedIn => User is not null;
    public static void Clear() { User = null; Company = null; }
}
