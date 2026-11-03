using System.Data;
using Inventory.Application.Interfaces;
using Inventory.Desktop.Seeding;
using Inventory.Domain.Catalog;
using Inventory.Domain.Entities;
using Inventory.Domain.Identity;
using Inventory.Domain.Inventory;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Desktop;

public static class DatabaseInitializer
{
    private static readonly Dictionary<string, string> CategoryCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Bebidas"] = "BEB",
        ["Cereales y desayuno"] = "CER",
        ["Golosinas"] = "GOL",
        ["Panadería"] = "PAN",
        ["Almacén"] = "ALM",
        ["Lácteos"] = "LAC",
        ["Limpieza"] = "LIM",
        ["Perfumería"] = "PER",
        ["Congelados"] = "CON",
        ["Carnes"] = "CAR",
        ["Frutas y Verduras"] = "FRU"
    };

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

        SeedCatalog(db, company.Id, admin.Id);
    }

    private static void SeedCatalog(AppDbContext db, int companyId, int adminId)
    {
        var catalog = DemoCatalog.Products;
        var created = new List<(Product Product, int Stock)>(catalog.Count);
        var categoryCounters = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        for (var index = 0; index < catalog.Count; index++)
        {
            var entry = catalog[index];

            if (!categoryCounters.TryGetValue(entry.Category, out var categoryNumber))
                categoryNumber = 0;

            categoryNumber++;
            categoryCounters[entry.Category] = categoryNumber;

            var categoryCode = CategoryCodes.TryGetValue(entry.Category, out var code)
                ? code
                : "GEN";

            var plu = (1001 + index).ToString("0000");
            var sku = $"{categoryCode}-{categoryNumber:000}";

            // Costo de referencia: 72% del precio de venta, con dos decimales.
            var cost = Math.Round(entry.Price * 0.72m, 2);

            var minimumStock = 4 + (index % 9);
            var stock = minimumStock + 6 + (index * 7 % 70);

            // Algunos productos quedan a propósito por debajo del mínimo
            // para poder probar la alerta de stock bajo.
            if (index % 13 == 0)
                stock = Math.Max(0, minimumStock - 2);

            var product = Product.Create(
                companyId,
                Plu.Create(plu),
                Sku.Create(sku),
                entry.Description,
                entry.Category,
                Money.Create(cost),
                Money.Create(entry.Price),
                minimumStock);

            db.Products.Add(product);
            created.Add((product, stock));
        }

        // Primer guardado: EF asigna los Id de todos los productos.
        db.SaveChanges();

        var stockItems = new List<StockItem>(created.Count);

        foreach (var (product, stock) in created)
        {
            var stockItem = StockItem.Create(product.Id, companyId);
            var movement = StockMovement.Receipt(
                companyId,
                product.Id,
                adminId,
                Quantity.CreateWhole(stock),
                reference: "Carga inicial de demostración");

            stockItem.AddMovement(movement);
            db.StockItems.Add(stockItem);
            db.StockMovements.Add(movement);
            stockItems.Add(stockItem);
        }

        db.SaveChanges();

        for (var index = 0; index < created.Count; index++)
            created[index].Product.LinkStockItem(stockItems[index].Id);

        db.SaveChanges();
    }

    /// <summary>
    /// La versión anterior no tenía PLU ni imagen de producto. Si detectamos
    /// un esquema viejo, recreamos la base demo para evitar columnas faltantes.
    /// En una app con datos productivos se usaría una migración.
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
                var hasImageColumn = false;

                while (reader.Read())
                {
                    var columnName = reader.GetString(1);
                    if (string.Equals(columnName, "Plu", StringComparison.OrdinalIgnoreCase))
                        hasPluColumn = true;
                    else if (string.Equals(columnName, "ImagePath", StringComparison.OrdinalIgnoreCase))
                        hasImageColumn = true;
                }

                incompatible = !hasPluColumn || !hasImageColumn;
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
