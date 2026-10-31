using Inventory.Application.DTOs;

namespace Inventory.Application.Interfaces;

public interface IAuthService
{
    LoginResponse Login(LoginRequest request);
}
