using Microsoft.EntityFrameworkCore;
using Library.Data.Entities;

namespace Library.Data;

public class LibraryDbContext(DbContextOptions<LibraryDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<InventoryItem> Inventory => Set<InventoryItem>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
    public DbSet<FulfillmentEvent> FulfillmentEvents => Set<FulfillmentEvent>();


    protected override void OnModelCreating(ModelBuilder b)
    {
        // Example of Fluent API to force any change into the EF database modeling
        b.Entity<Product>(e =>
        {
            // making an index
            e.HasIndex(p => p.Sku).IsUnique();

            // Setting decimal places in price
            e.Property(p => p.Price).HasColumnType("decimal(10,2)");

            // Setting a relationship
            e.HasOne(p => p.Inventory)
                .WithOne(i => i.Product)
                .HasForeignKey<InventoryItem>(i => i.ProductId);
        });


        b.Entity<InventoryItem>().Property(i => i.RowVersion).IsRowVersion();

        b.Entity<Customer>().Property(c => c.Email).HasMaxLength(256);
        b.Entity<Customer>().HasIndex(c => c.Email).IsUnique();

        // Here you should provide the ids even if they are created as identity
        b.Entity<Product>().HasData(
            new Product {Id = 1, Sku = "BK-001", Name = "Clean Code", Price = 32 },
            new Product {Id = 2, Sku = "BK-002", Name = "The Pragmatic Programmer", Price = 38 },
            new Product {Id = 3, Sku = "BK-003", Name = "Refactoring", Price = 45 }
        );
        b.Entity<InventoryItem>().HasData(
            new InventoryItem {Id = 1, ProductId = 1, CurrentStock = 5},
            new InventoryItem {Id = 2, ProductId = 2, CurrentStock = 3},
            new InventoryItem {Id = 3, ProductId = 3, CurrentStock = 8}
        );

        b.Entity<Customer>().HasData(
            new Customer {Id = 1, Name = "Ada Lovelace", Email = "ada@example.com"},
            new Customer {Id = 2, Name = "Alan Turing", Email = "alan@example.com"}
        );
    }
}