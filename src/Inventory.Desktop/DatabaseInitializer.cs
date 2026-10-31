using System.Linq;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Persistence;

namespace Inventory.Desktop;

public static class DatabaseInitializer
{
    public static void EnsureSeed(AppDbContext db, IPasswordHasher hasher)
    {
        db.Database.EnsureCreated();

        if (!db.Companies.Any())
        {
            var company = new Company { Name = "Demo Company S.A." };
            db.Companies.Add(company);
            db.SaveChanges();

            var admin = new User
            {
                CompanyId = company.Id,
                Username = "admin",
                PasswordHash = hasher.Hash("123456"),
                Role = "Admin",
                IsActive = true
            };
            db.Users.Add(admin);

            db.Products.AddRange(
                new Product { CompanyId = company.Id, Sku = "P001", Name = "Laptop HP", Category = "Tecnología", Price = 1200m, Stock = 5 },
                new Product { CompanyId = company.Id, Sku = "P002", Name = "Silla Ergonómica", Category = "Muebles", Price = 350m, Stock = 12 },
                new Product { CompanyId = company.Id, Sku = "P003", Name = "Teclado Mecánico", Category = "Tecnología", Price = 89.90m, Stock = 25 },
                new Product { CompanyId = company.Id, Sku = "P004", Name = "Monitor 27''", Category = "Tecnología", Price = 450m, Stock = 8 },
                new Product { CompanyId = company.Id, Sku = "P005", Name = "Cuaderno Rayado", Category = "Papelería", Price = 4.50m, Stock = 100 }
            );
            db.SaveChanges();
        }
    }
}
