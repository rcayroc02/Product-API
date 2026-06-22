using Microsoft.EntityFrameworkCore;
using TuProyecto.Models;

namespace TuProyecto.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Relación Category -> Products
    modelBuilder.Entity<Product>()
        .HasOne(p => p.Category)
        .WithMany(c => c.Products)
        .HasForeignKey(p => p.CategoryId)
        .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<Product>()
        .Property(p => p.Price)
        .HasColumnType("decimal(18,2)");

    // INITIAL DATA - CATEGORIES
    modelBuilder.Entity<Category>().HasData(
        new Category { Id = 1, Name = "Electrónica", Description = "Dispositivos electrónicos y gadgets" },
        new Category { Id = 2, Name = "Ropa", Description = "Prendas de vestir y accesorios" },
        new Category { Id = 3, Name = "Alimentos", Description = "Productos alimenticios y abarrotes" }
    );

    // INITIAL DATA - PRODUCTS
   // INITIAL DATA - PRODUCTS
    // INITIAL DATA - PRODUCTS
    modelBuilder.Entity<Product>().HasData(
        // Electrónica (Id = 1)
        new Product { Id = 1, Name = "Laptop HP Pavillion", Price = 899.99m, Stock = 10, CategoryId = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
        new Product { Id = 4, Name = "Audífonos Sony WH-CH520", Price = 45.00m, Stock = 25, CategoryId = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
        new Product { Id = 5, Name = "Mouse Inalámbrico Logitech", Price = 19.99m, Stock = 40, CategoryId = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
        new Product { Id = 6, Name = "Monitor Gamer 24'' ASUS", Price = 179.50m, Stock = 8, CategoryId = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

        // Ropa (Id = 2)
        new Product { Id = 2, Name = "Polo Nike Park", Price = 35.00m, Stock = 50, CategoryId = 2, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
        new Product { Id = 7, Name = "Casaca Impermeable Columbia", Price = 120.00m, Stock = 15, CategoryId = 2, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
        new Product { Id = 8, Name = "Medias Deportivas Adidas (3 pack)", Price = 12.50m, Stock = 100, CategoryId = 2, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

        // Alimentos (Id = 3)
        new Product { Id = 3, Name = "Arroz Extra 1kg", Price = 3.50m, Stock = 200, CategoryId = 3, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
        new Product { Id = 9, Name = "Aceite de Oliva Virgen 500ml", Price = 14.20m, Stock = 60, CategoryId = 3, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
        new Product { Id = 10, Name = "Café Altomayo Gourmet 250g", Price = 8.50m, Stock = 85, CategoryId = 3, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
    );
}
    }
}