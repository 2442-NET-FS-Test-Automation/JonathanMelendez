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
    public DbSet<DishIngredient> DishIngredients => Set<DishIngredient>();
    public DbSet<FulfillmentEvent> FulfillmentEvents => Set<FulfillmentEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Ingredient>().Property(i => i.RowVersion).IsRowVersion();

        modelBuilder.Entity<Order>().Property(o => o.CreatedUtc)
            .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<FulfillmentEvent>().Property(f => f.FulfilledAtUtc)
            .HasDefaultValueSql("GETUTCDATE()");

        // Seed Customers
        modelBuilder.Entity<Customer>().HasData(
            new Customer {Id = 1, Name = "Jonathan", Email = "jonathan@example.com"},
            new Customer {Id = 2, Name = "Paula", Email = "paula@example.com"},
            new Customer {Id = 3, Name = "Takis", Email = "takis@example.com"},
            new Customer {Id = 4, Name = "Lemon", Email = "lemon@example.com"},
            new Customer {Id = 5, Name = "Bahji", Email = "bhy@example.com"}
        );

        // Seed Ingredients
        modelBuilder.Entity<Ingredient>().HasData(
            new Ingredient {Id = 1, Name = "White Rice", Unit = Units.kg, Stock = 5},
            new Ingredient {Id = 2, Name = "Water", Unit = Units.l, Stock = 20},
            new Ingredient {Id = 3, Name = "Salt", Unit = Units.kg, Stock = 1},
            new Ingredient {Id = 4, Name = "Milk", Unit = Units.l, Stock = 5},
            new Ingredient {Id = 5, Name = "Butter", Unit = Units.kg, Stock = 1},
            new Ingredient {Id = 6, Name = "Sugar", Unit = Units.kg, Stock = 2},
            new Ingredient {Id = 7, Name = "Condensed Milk", Unit = Units.oz, Stock = 50},
            new Ingredient {Id = 8, Name = "Egg", Unit = Units.pz, Stock = 50},
            new Ingredient {Id = 9, Name = "Flour", Unit = Units.kg, Stock = 5},
            new Ingredient {Id = 10, Name = "Chicken", Unit = Units.kg, Stock = 5},
            new Ingredient {Id = 11, Name = "Cheese", Unit = Units.kg, Stock = 8},
            new Ingredient {Id = 12, Name = "Potato", Unit = Units.kg, Stock = 6},
            new Ingredient {Id = 13, Name = "Tomato", Unit = Units.pz, Stock = 15},
            new Ingredient {Id = 14, Name = "Ground Beef", Unit = Units.kg, Stock = 3},
            new Ingredient {Id = 15, Name = "Onion", Unit = Units.pz, Stock = 10},
            new Ingredient {Id = 16, Name = "Cream", Unit = Units.kg, Stock = 5},
            new Ingredient {Id = 17, Name = "Pasta", Unit = Units.kg, Stock = 3}
        );

        // Seed Dishes
        modelBuilder.Entity<Dish>().HasData(
            new Dish {Id = 1, Name = "Leche Asada", Description = "Traditional Latin American custard dessert popular in countries like Peru, Chile, and Colombia. Similar to flan or crème brûlée.", 
                OriginCountry = "Chile", Enabled = true, Price = 5.99m},
            new Dish {Id = 2, Name = "Colombian Buñuelos", Description = "Round, deep-fried cheese fritters that are crispy on the outside and soft and airy on the inside.", 
                OriginCountry = "Colombia", Enabled = true, Price = 4.99m},
            new Dish {Id = 3, Name = "Arroz con leche", Description = "Traditional, creamy rice pudding", 
                OriginCountry = "Spain", Enabled = true, Price = 3.99m},
            new Dish {Id = 4, Name = "Fried cheese empanadas", Description = "Classic Latin American pastry made by folding dough into a half-moon shape, filling it with melty cheese, and deep-frying it until the exterior is crispy and the inside is gooey.", 
                OriginCountry = "Mexico", Enabled = true, Price = 6.99m},
            new Dish {Id = 5, Name = "New York cheesecake", Description = "Iconic, ultra-rich dessert renowned for its dense, velvety texture.", 
                OriginCountry = "United States", Enabled = true, Price = 7.99m},
            new Dish {Id = 6, Name = "Fettucine alfredo", Description = "Rich, comforting pasta dish consisting of long, flat fettuccine noodles coated in a velvety, indulgent sauce.", 
                OriginCountry = "Italy", Enabled = true, Price = 8.99m},
            new Dish {Id = 7, Name = "Potato Pie", Description = "Savory Latin American casserole. It features a base of seasoned ground beef (pino) layered with hard-boiled eggs, black olives, and raisins.", 
                OriginCountry = "Chile", Enabled = true, Price = 5.99m},
            new Dish {Id = 8, Name = "Bitterballen", Description = "Traditional Dutch bite-sized snacks consisting of a rich, thick meat ragout enveloped in a crispy, deep-fried breadcrumb crust.", 
                OriginCountry = "Netherlands", Enabled = true, Price = 4.99m},
            new Dish {Id = 9, Name = "Fatteh", Description = "Classic Middle Eastern dish. It features crispy, toasted pita bread combined with warm ingredients and creamy sauces.", 
                OriginCountry = "Egypt", Enabled = true, Price = 5.99m},
            new Dish {Id = 10, Name = "Kentucky Fried Chicken", Description = "Southern-style, pressure-fried chicken. Each piece is coated in a proprietary blend of 11 herbs and spices, resulting in a signature golden-brown, crispy exterior that shatters upon the first bite, giving way to piping-hot, exceptionally tender and juicy meat.", 
                OriginCountry = "United States", Enabled = true, Price = 7.99m}
        );
        modelBuilder.Entity<DishIngredient>().HasData(
            new DishIngredient {Id = 1, DishId = 1, IngredientId = 4, Quantity = 0.25m},
            new DishIngredient {Id = 2, DishId = 1, IngredientId = 8, Quantity = 4},
            new DishIngredient {Id = 3, DishId = 1, IngredientId = 6, Quantity = 0.2m},
            new DishIngredient {Id = 4, DishId = 2, IngredientId = 8, Quantity = 1},
            new DishIngredient {Id = 5, DishId = 2, IngredientId = 11, Quantity = 0.2m},
            new DishIngredient {Id = 6, DishId = 2, IngredientId = 6, Quantity = 0.1m},
            new DishIngredient {Id = 7, DishId = 2, IngredientId = 3, Quantity = 0.002m},
            new DishIngredient {Id = 8, DishId = 2, IngredientId = 4, Quantity = 0.1m},
            new DishIngredient {Id = 9, DishId = 3, IngredientId = 1, Quantity = 0.2m},
            new DishIngredient {Id = 10, DishId = 3, IngredientId = 2, Quantity = 0.4m},
            new DishIngredient {Id = 11, DishId = 3, IngredientId = 4, Quantity = 0.8m},
            new DishIngredient {Id = 12, DishId = 3, IngredientId = 3, Quantity = 0.001m},
            new DishIngredient {Id = 13, DishId = 3, IngredientId = 5, Quantity = 0.05m},
            new DishIngredient {Id = 14, DishId = 3, IngredientId = 6, Quantity = 0.2m},
            new DishIngredient {Id = 15, DishId = 3, IngredientId = 7, Quantity = 12},
            new DishIngredient {Id = 16, DishId = 4, IngredientId = 9, Quantity = 0.6m},
            new DishIngredient {Id = 17, DishId = 4, IngredientId = 3, Quantity = 0.05m},
            new DishIngredient {Id = 18, DishId = 4, IngredientId = 4, Quantity = 0.02m},
            new DishIngredient {Id = 19, DishId = 4, IngredientId = 8, Quantity = 1},
            new DishIngredient {Id = 20, DishId = 4, IngredientId = 11, Quantity = 0.4m},
            new DishIngredient {Id = 21, DishId = 5, IngredientId = 5, Quantity = 0.1m},
            new DishIngredient {Id = 22, DishId = 5, IngredientId = 6, Quantity = 0.4m},
            new DishIngredient {Id = 23, DishId = 5, IngredientId = 11, Quantity = 1},
            new DishIngredient {Id = 24, DishId = 5, IngredientId = 9, Quantity = 0.1m},
            new DishIngredient {Id = 25, DishId = 5, IngredientId = 8, Quantity = 3},
            new DishIngredient {Id = 26, DishId = 6, IngredientId = 16, Quantity = 0.3m},
            new DishIngredient {Id = 27, DishId = 6, IngredientId = 5, Quantity = 0.1m},
            new DishIngredient {Id = 28, DishId = 6, IngredientId = 9, Quantity = 0.01m},
            new DishIngredient {Id = 29, DishId = 6, IngredientId = 11, Quantity = 0.1m},
            new DishIngredient {Id = 30, DishId = 6, IngredientId = 17, Quantity = 0.25m},
            new DishIngredient {Id = 31, DishId = 7, IngredientId = 12, Quantity = 1},
            new DishIngredient {Id = 32, DishId = 7, IngredientId = 5, Quantity = 0.1m},
            new DishIngredient {Id = 33, DishId = 7, IngredientId = 3, Quantity = 0.02m},
            new DishIngredient {Id = 34, DishId = 7, IngredientId = 8, Quantity = 2},
            new DishIngredient {Id = 35, DishId = 7, IngredientId = 15, Quantity = 1},
            new DishIngredient {Id = 36, DishId = 7, IngredientId = 13, Quantity = 3},
            new DishIngredient {Id = 37, DishId = 7, IngredientId = 14, Quantity = 0.3m},
            new DishIngredient {Id = 38, DishId = 8, IngredientId = 5, Quantity = 0.1m},
            new DishIngredient {Id = 39, DishId = 8, IngredientId = 9, Quantity = 0.2m},
            new DishIngredient {Id = 40, DishId = 8, IngredientId = 15, Quantity = 0.5m},
            new DishIngredient {Id = 41, DishId = 8, IngredientId = 14, Quantity = 0.5m},
            new DishIngredient {Id = 42, DishId = 8, IngredientId = 3, Quantity = 0.03m},
            new DishIngredient {Id = 43, DishId = 8, IngredientId = 8, Quantity = 2},
            new DishIngredient {Id = 44, DishId = 9, IngredientId = 14, Quantity = 0.3m},
            new DishIngredient {Id = 45, DishId = 9, IngredientId = 15, Quantity = 1},
            new DishIngredient {Id = 46, DishId = 9, IngredientId = 13, Quantity = 2},
            new DishIngredient {Id = 47, DishId = 9, IngredientId = 1, Quantity = 0.5m},
            new DishIngredient {Id = 48, DishId = 9, IngredientId = 5, Quantity = 0.1m},
            new DishIngredient {Id = 49, DishId = 9, IngredientId = 3, Quantity = 0.01m},
            new DishIngredient {Id = 50, DishId = 10, IngredientId = 10, Quantity = 2},
            new DishIngredient {Id = 51, DishId = 10, IngredientId = 8, Quantity = 1},
            new DishIngredient {Id = 52, DishId = 10, IngredientId = 9, Quantity = 0.5m},
            new DishIngredient {Id = 53, DishId = 10, IngredientId = 6, Quantity = 0.01m},
            new DishIngredient {Id = 54, DishId = 10, IngredientId = 3, Quantity = 0.01m}
        );
    
        // Seed Orders
        modelBuilder.Entity<Order>().HasData(
            new Order {Id = 1, CustomerId = 3, Priority = OrderPriority.Normal, Status = OrderStatus.Fulfilled, CompletedUtc = new DateTime(2026, 07, 07, 20, 15, 07, DateTimeKind.Utc)},
            new Order {Id = 2, CustomerId = 2, Priority = OrderPriority.Normal, Status = OrderStatus.Backordered, CompletedUtc = new DateTime(2026, 07, 07, 20, 33, 21, DateTimeKind.Utc)},
            new Order {Id = 3, CustomerId = 5, Priority = OrderPriority.Urgent, Status = OrderStatus.Fulfilled, CompletedUtc = new DateTime(2026, 07, 07, 21, 11, 35, DateTimeKind.Utc)}
        );
        modelBuilder.Entity<OrderLine>().HasData(
            new OrderLine {Id = 1, OrderId = 1, DishId = 5, Quantity = 2},
            new OrderLine {Id = 2, OrderId = 2, DishId = 6, Quantity = 1},
            new OrderLine {Id = 3, OrderId = 2, DishId = 1, Quantity = 1},
            new OrderLine {Id = 4, OrderId = 3, DishId = 4, Quantity = 1},
            new OrderLine {Id = 5, OrderId = 3, DishId = 8, Quantity = 3}
        );
    
        // Seed FulfillmentEvents
        modelBuilder.Entity<FulfillmentEvent>().HasData(
            new FulfillmentEvent {Id = 1, OrderId = 1, Result = FulfillmentResult.Fulfilled, FulfilledAtUtc = new DateTime(2026, 07, 07, 20, 15, 11, DateTimeKind.Utc)},
            new FulfillmentEvent {Id = 2, OrderId = 2, Result = FulfillmentResult.Backordered, FulfilledAtUtc = new DateTime(2026, 07, 07, 20, 33, 25, DateTimeKind.Utc)},
            new FulfillmentEvent {Id = 3, OrderId = 3, Result = FulfillmentResult.Fulfilled, FulfilledAtUtc = new DateTime(2026, 07, 07, 21, 11, 57, DateTimeKind.Utc)}
        );
    }
}