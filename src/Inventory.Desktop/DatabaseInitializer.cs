using System.Data;
using Inventory.Application.Interfaces;
using Inventory.Domain.Catalog;
using Inventory.Domain.Entities;
using Inventory.Domain.Identity;
using Inventory.Domain.Inventory;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Desktop;

public static class DatabaseInitializer
{
    public static void EnsureSeed(AppDbContext db, IPasswordHasher hasher)
    {
        EnsureCompatibleSchema(db);
        db.Database.EnsureCreated();

        if (db.Companies.Any())
            return;

        var company = new Company { Name = "Demo Company S.A." };
        db.Companies.Add(company);
        db.SaveChanges();

        var admin = User.Create(
            company.Id,
            EmployeeNumber.Create(1),
            JobTitle.Create("Administrador de Sistemas"),
            "admin",
            hasher.Hash("123456"),
            UserRole.Admin,
            isActive: true);

        var ezequiel = User.Create(
            company.Id,
            EmployeeNumber.Create(102),
            JobTitle.Create("Programador"),
            "ecano",
            hasher.Hash("123456"),
            UserRole.Employee,
            isActive: true);

        db.Users.AddRange(admin, ezequiel);
        db.SaveChanges();

        var products = new (string Plu, string Sku, string Description, string Category,
            decimal Cost, decimal Price, int MinimumStock, int Stock)[]
        {
            ("1001", "TEC-001", "Laptop HP ProBook 450", "Tecnología", 950m, 1200m, 3, 8),
            ("1002", "TEC-002", "Monitor LED 27\" Full HD", "Tecnología", 320m, 450m, 4, 12),
            ("1003", "TEC-003", "Teclado mecánico RGB", "Tecnología", 55m, 89.90m, 6, 25),
            ("1004", "TEC-004", "Mouse inalámbrico ergonómico", "Tecnología", 18m, 32.50m, 10, 40),
            ("2001", "MUE-001", "Silla ergonómica con apoyabrazos", "Muebles", 210m, 350m, 3, 9),
            ("2002", "MUE-002", "Escritorio regulable 140 cm", "Muebles", 380m, 590m, 2, 4),
            ("2003", "MUE-003", "Archivador metálico 4 cajones", "Muebles", 150m, 245m, 2, 6),
            ("3001", "PAP-001", "Resma de papel A4 500 hojas", "Papelería", 3.20m, 5.80m, 20, 90),
            ("3002", "PAP-002", "Cuaderno rayado universitario", "Papelería", 2.10m, 4.50m, 25, 110),
            ("3003", "PAP-003", "Bolígrafo tinta azul", "Papelería", 0.45m, 1.20m, 50, 300),
            ("4001", "LIM-001", "Limpiador multiuso 5 L", "Limpieza", 4.80m, 8.90m, 10, 18),
            ("4002", "LIM-002", "Alcohol en gel 500 ml", "Limpieza", 1.90m, 3.75m, 15, 7)
        };

        foreach (var (plu, sku, description, category, cost, price, minimumStock, stock) in products)
        {
            var product = Product.Create(
                company.Id,
                Plu.Create(plu),
                Sku.Create(sku),
                description,
                category,
                Money.Create(cost),
                Money.Create(price),
                minimumStock);

            db.Products.Add(product);
            db.SaveChanges();

            var stockItem = StockItem.Create(product.Id, company.Id);
            var movement = StockMovement.Receipt(
                company.Id,
                product.Id,
                admin.Id,
                Quantity.CreateWhole(stock),
                reference: "Carga inicial de demostración");

            stockItem.AddMovement(movement);
            db.StockItems.Add(stockItem);
            db.StockMovements.Add(movement);
            db.SaveChanges();

            product.LinkStockItem(stockItem.Id);
            db.SaveChanges();
        }
    }

    /// <summary>
    /// La versión anterior guardaba Product.Name/Price/Stock en la misma tabla.
    /// Si detectamos ese esquema viejo, recreamos la base demo para evitar
    /// columnas faltantes. En una app con datos productivos se usaría una migración.
    /// </summary>
    private static void EnsureCompatibleSchema(AppDbContext db)
    {
        var connection = db.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;

        if (shouldClose)
            connection.Open();

        var incompatible = false;

        try
        {
            using var tableCommand = connection.CreateCommand();
            tableCommand.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Products'";

            var productsTableExists = tableCommand.ExecuteScalar() is not null;
            if (productsTableExists)
            {
                using var columnsCommand = connection.CreateCommand();
                columnsCommand.CommandText = "PRAGMA table_info('Products')";

                using var reader = columnsCommand.ExecuteReader();
                var hasPluColumn = false;

                while (reader.Read())
                {
                    if (string.Equals(reader.GetString(1), "Plu", StringComparison.OrdinalIgnoreCase))
                    {
                        hasPluColumn = true;
                        break;
                    }
                }

                incompatible = !hasPluColumn;
            }
        }
        finally
        {
            if (shouldClose)
                connection.Close();
        }

        if (incompatible)
            db.Database.EnsureDeleted();
    }
}
