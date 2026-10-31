using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Company> Companies { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }

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
            e.HasMany(c => c.Users).WithOne(u => u.Company).HasForeignKey(u => u.CompanyId);
            e.HasMany(c => c.Products).WithOne(p => p.Company).HasForeignKey(p => p.CompanyId);
        });

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Username).IsRequired().HasMaxLength(100);
            e.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
            e.Property(u => u.Role).IsRequired().HasMaxLength(50);
            e.HasIndex(u => new { u.Username }).IsUnique();
        });

        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Sku).IsRequired().HasMaxLength(50);
            e.Property(p => p.Name).IsRequired().HasMaxLength(200);
            e.Property(p => p.Category).IsRequired().HasMaxLength(100);
            e.Property(p => p.Price).HasColumnType("decimal(18,2)");
            e.HasIndex(p => new { p.Sku, p.CompanyId }).IsUnique();
        });
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries();
        foreach (var entry in entries)
        {
            if (entry.Entity is User u && entry.State == Microsoft.EntityFrameworkCore.EntityState.Added)
                u.CreatedAt = DateTime.UtcNow;
            if (entry.Entity is Product p && entry.State == Microsoft.EntityFrameworkCore.EntityState.Added)
                p.CreatedAt = DateTime.UtcNow;
        }
        return base.SaveChanges();
    }
}
