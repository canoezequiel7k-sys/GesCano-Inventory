using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Domain.Exceptions;
using Inventory.Domain.Interfaces;

namespace Inventory.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(
        IUserRepository userRepository,
        ICompanyRepository companyRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _companyRepository = companyRepository;
        _passwordHasher = passwordHasher;
    }

    public LoginResponse Login(LoginRequest request)
    {
        var user = _userRepository.GetByUsername(request.Username);

        if (user is null || !user.IsActive)
            throw new InvalidCredentialsException();

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new InvalidCredentialsException();

        var company = _companyRepository.GetById(user.CompanyId)
            ?? throw new EntityNotFoundException("Empresa", user.CompanyId);

        return new LoginResponse(
            user.Id,
            user.Username,
            company.Id,
            company.Name,
            user.Role);
    }
}
