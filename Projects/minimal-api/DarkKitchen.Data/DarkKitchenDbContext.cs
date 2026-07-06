using Microsoft.EntityFrameworkCore;
using DarkKitchen.Data.Entities;

namespace DarkKitchen.Data;

public class DarkKitchenDbContext : DbContext
{
    public DarkKitchenDbContext(DbContextOptions<DarkKitchenDbContext> options) : base(options) { }   

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
    public DbSet<Dish> Dishes => Set<Dish>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<FulfillmentEvent> FulfillmentEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Ingredient>().Property(i => i.RowVersion).IsRowVersion();
    }
}