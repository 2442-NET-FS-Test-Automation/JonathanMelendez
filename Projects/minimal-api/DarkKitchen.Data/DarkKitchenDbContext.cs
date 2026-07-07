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

        // Seed initial customers
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
            new Ingredient {Id = 10, Name = "Vegetable Oil", Unit = Units.l, Stock = 5},
            new Ingredient {Id = 11, Name = "Cheese", Unit = Units.kg, Stock = 8},
            new Ingredient {Id = 12, Name = "Potato", Unit = Units.kg, Stock = 6},
            new Ingredient {Id = 13, Name = "Tomato", Unit = Units.kg, Stock = 2},
            new Ingredient {Id = 14, Name = "Ground Beef", Unit = Units.kg, Stock = 3},
            new Ingredient {Id = 15, Name = "Onion", Unit = Units.pz, Stock = 10}
        );

        // Seed Dishes
        modelBuilder.Entity<Dish>().HasData(
            new Dish {Id = 1, Name = "Leche Asada", Description = "Traditional Latin American custard dessert popular in countries like Peru, Chile, and Colombia. Similar to flan or crème brûlée.", 
                OriginCountry = "Chile", Enabled = true,
                Ingredients = [
                    new DishIngredient {Id = 1, DishId = 1, IngredientId = 4, Quantity = 0.25m},
                    new DishIngredient {Id = 2, DishId = 1, IngredientId = 8, Quantity = 4},
                    new DishIngredient {Id = 3, DishId = 1, IngredientId = 6, Quantity = 0.2m}
                ]},
            new Dish {Id = 2, Name = "Colombian Buñuelos", Description = "Round, deep-fried cheese fritters that are crispy on the outside and soft and airy on the inside.", 
                OriginCountry = "Colombia", Enabled = true,
                Ingredients = [
                    new DishIngredient {Id = 4, DishId = 2, IngredientId = 8, Quantity = 1},
                    new DishIngredient {Id = 5, DishId = 2, IngredientId = 11, Quantity = 0.2m},
                    new DishIngredient {Id = 6, DishId = 2, IngredientId = 6, Quantity = 0.1m},
                    new DishIngredient {Id = 7, DishId = 2, IngredientId = 3, Quantity = 0.002m},
                    new DishIngredient {Id = 8, DishId = 2, IngredientId = 4, Quantity = 0.1m}
            ]},
            new Dish {Id = 3, Name = "Arroz con leche", Description = "Traditional, creamy rice pudding", 
                OriginCountry = "Spain", Enabled = true,
                Ingredients = [
                    new DishIngredient {Id = 9, DishId = 3, IngredientId = 1, Quantity = 0.2m},
                    new DishIngredient {Id = 10, DishId = 3, IngredientId = 2, Quantity = 0.4m},
                    new DishIngredient {Id = 11, DishId = 3, IngredientId = 4, Quantity = 0.8m},
                    new DishIngredient {Id = 12, DishId = 3, IngredientId = 3, Quantity = 0.001m},
                    new DishIngredient {Id = 13, DishId = 3, IngredientId = 5, Quantity = 0.05m},
                    new DishIngredient {Id = 14, DishId = 3, IngredientId = 6, Quantity = 0.2m},
                    new DishIngredient {Id = 14, DishId = 3, IngredientId = 7, Quantity = 12},
            ]},
            new Dish {Id = 4, Name = "Fried cheese empanadas", Description = "Classic Latin American pastry made by folding dough into a half-moon shape, filling it with melty cheese, and deep-frying it until the exterior is crispy and the inside is gooey.", 
                OriginCountry = "Mexico", Enabled = true,
                Ingredients = [
                    new DishIngredient {Id = 15, DishId = 4, IngredientId = 9, Quantity = 0.6m},
                    new DishIngredient {Id = 16, DishId = 4, IngredientId = 3, Quantity = 0.05m},
                    new DishIngredient {Id = 17, DishId = 4, IngredientId = 4, Quantity = 0.02m},
                    new DishIngredient {Id = 18, DishId = 4, IngredientId = 8, Quantity = 1},
                    new DishIngredient {Id = 19, DishId = 4, IngredientId = 11, Quantity = 0.4m}
            ]},
            new Dish {Id = 5, Name = "New York cheesecake", Description = "Iconic, ultra-rich dessert renowned for its dense, velvety texture.", 
                OriginCountry = "United States", Enabled = true,
                Ingredients = [
                    new DishIngredient {Id = 20, DishId = 5, IngredientId = 5, Quantity = 0.1m},
                    new DishIngredient {Id = 21, DishId = 5, IngredientId = 6, Quantity = 0.4m},
                    new DishIngredient {Id = 22, DishId = 5, IngredientId = 11, Quantity = 1},
                    new DishIngredient {Id = 23, DishId = 5, IngredientId = 9, Quantity = 0.1m},
                    new DishIngredient {Id = 24, DishId = 5, IngredientId = 8, Quantity = 3}
            ]}
        );
    }
}