using Inventory.Domain.Catalog;
using Inventory.Domain.Entities;
using Inventory.Domain.Identity;
using Inventory.Domain.Inventory;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockItem> StockItems => Set<StockItem>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "inventory.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).IsRequired().HasMaxLength(100);

            e.HasMany(c => c.Users)
                .WithOne(u => u.Company)
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(c => c.Products)
                .WithOne(p => p.Company)
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Username).IsRequired().HasMaxLength(100);
            e.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
            e.Property(u => u.Role).HasConversion<string>().IsRequired().HasMaxLength(20);
            e.Property(u => u.EmployeeNumber)
                .HasConversion(v => v.Value, v => EmployeeNumber.Create(v))
                .IsRequired();
            e.Property(u => u.JobTitle)
                .HasConversion(v => v.Value, v => JobTitle.Create(v))
                .IsRequired()
                .HasMaxLength(100);

            e.HasIndex(u => u.Username).IsUnique();
            e.HasIndex(u => new { u.CompanyId, u.EmployeeNumber }).IsUnique();
        });

        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Plu)
                .HasConversion(v => v.Value, v => Plu.Create(v))
                .IsRequired()
                .HasMaxLength(8);
            e.Property(p => p.Sku)
                .HasConversion(v => v.Value, v => Sku.Create(v))
                .IsRequired()
                .HasMaxLength(32);
            e.Property(p => p.Description).IsRequired().HasMaxLength(200);
            e.Property(p => p.Category).IsRequired().HasMaxLength(100);
            e.Property(p => p.UnitCost)
                .HasConversion(v => v.Value, v => Money.Create(v))
                .HasColumnType("decimal(18,2)");
            e.Property(p => p.UnitPrice)
                .HasConversion(v => v.Value, v => Money.Create(v))
                .HasColumnType("decimal(18,2)");
            e.Property(p => p.MinimumStock).IsRequired();
            e.Property(p => p.ImagePath).HasMaxLength(300);

            e.HasIndex(p => new { p.Sku, p.CompanyId }).IsUnique();
            e.HasIndex(p => new { p.Plu, p.CompanyId }).IsUnique();
        });

        modelBuilder.Entity<StockItem>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.CurrentStock)
                .HasConversion(v => v.Value, v => Quantity.Create(v, allowFractions: true))
                .HasColumnType("decimal(18,4)");
            e.Property(s => s.ConcurrencyToken).IsRequired();

            e.Ignore(s => s.Movements);
            e.Ignore(s => s.DomainEvents);

            e.HasIndex(s => s.ProductId).IsUnique();
            e.HasIndex(s => s.CompanyId);
        });

        modelBuilder.Entity<StockMovement>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.Type).HasConversion<string>().IsRequired().HasMaxLength(20);
            e.Property(m => m.Quantity)
                .HasConversion(v => v.Value, v => Quantity.Create(v, allowFractions: true))
                .HasColumnType("decimal(18,4)");
            e.Property(m => m.OccurredAt).IsRequired();
            e.Property(m => m.Reference).HasMaxLength(150);
            e.Property(m => m.Reason).HasMaxLength(250);

            e.Ignore(m => m.DomainEvents);

            e.HasIndex(m => new { m.CompanyId, m.ProductId, m.OccurredAt });
        });
    }
}
