using Inventory.Application.Interfaces;
using Inventory.Application.Services;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Services;

namespace Inventory.Desktop;

public static class Di
{
    public static AppDbContext Db { get; } = new();
    public static IPasswordHasher PasswordHasher { get; } = new BCryptPasswordHasher();
    public static ICompanyRepository CompanyRepository { get; } = new CompanyRepository(Db);
    public static IUserRepository UserRepository { get; } = new UserRepository(Db);
    public static IProductRepository ProductRepository { get; } = new ProductRepository(Db);
    public static IStockItemRepository StockItemRepository { get; } = new StockItemRepository(Db);
    public static IAuthService AuthService { get; } = new AuthService(UserRepository, CompanyRepository, PasswordHasher);
    public static ICompanyService CompanyService { get; } = new CompanyService(CompanyRepository);
    public static IProductService ProductService { get; } = new ProductService(ProductRepository, StockItemRepository);
}
