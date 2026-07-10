using Microsoft.EntityFrameworkCore;
using Serilog;

using DarkKitchen.Data.Defaults;
using DarkKitchen.Data.Entities;
using DarkKitchen.Data.Exceptions;

namespace DarkKitchen.Data.Repository;

public class DarkKitchenRepoSqlServer(IDbContextFactory<DarkKitchenDbContext> dbF) : IDarkKitchenRepo
{
    private readonly IDbContextFactory<DarkKitchenDbContext> _dbF = dbF;
    
    // Orders
    public async Task<Order?> GetOrderByIdAsync(int orderId, CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        return await db.Orders
            .Include(o => o.Lines)
            .ThenInclude(l => l.Dish)
            .FirstOrDefaultAsync(o => o.Id == orderId, ct);
    }
    public async Task<List<Order>> GetOrdersByIdsAsync(IEnumerable<int> orderIds, CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        return await db.Orders
            .Where(o => orderIds.Contains(o.Id))
            .ToListAsync(ct);
    }
    public async Task<List<Order>> GetOrdersForCustomerAsync(int customerId, CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        return await db.Orders
            .Where(o => o.CustomerId == customerId)
            .Include(o => o.Lines)
            .ThenInclude(l => l.Dish)
            .ToListAsync(ct);
    }
    public async Task<List<Order>> GetAllOrdersAsync(CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        return await db.Orders
            .Include(o => o.Lines)
            .ThenInclude(l => l.Dish)
            .ToListAsync(ct);
    }
    public async Task AddOrderAsync(Order order, CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        await db.Orders.AddAsync(order, ct);
        await db.SaveChangesAsync(ct);
        Log.Information("Added order with id {id}", order.Id);
    }
    public async Task AddOrdersAsync(IEnumerable<Order> orders, CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        await db.Orders.AddRangeAsync(orders, ct);
        await db.SaveChangesAsync(ct);
        Log.Information("Added {qty} orders", orders.Count());
    }

    // Inventory
    public async Task<List<Ingredient>> GetAllIngredientsAsync(CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        return await db.Ingredients.ToListAsync(ct);
    }
    public async Task<Ingredient?> GetIngredientByIdAsync(int id, CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        return await db.Ingredients.Where(i => i.Id == id).FirstOrDefaultAsync(ct);
    }
    public async Task<List<Ingredient>> GetIngredientsByNameAsync(string searchedName, CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        return await db.Ingredients.Where(i => i.Name.ToLower().Contains(searchedName.ToLower())).ToListAsync(ct);
    }
    public async Task<List<Ingredient>> GetIngredientsBelowStockAsync(decimal minStock, CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        return await db.Ingredients.Where(i => i.Stock < minStock).ToListAsync(ct);
    }
    public async Task IngredientsResetStock(CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        foreach (var ingredient in db.Ingredients)
        {
            if (IngredientDefaults.InitialStocks.TryGetValue(ingredient.Id, out decimal initialStock))
            {
                ingredient.Stock = initialStock;
                Log.Information("Reset ingredient {id} to {stock} stock", ingredient.Id, initialStock);
            }
        }
        Log.Information("Applying reset changes...");
        await db.SaveChangesAsync(ct);
    }

    // Dishes
    public async Task<List<Dish>> GetAllDishesAsync(CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        return await db.Dishes.ToListAsync(ct);
    }
    public async Task<Dish?> GetDishByIdAsync(int dishId, CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        return await db.Dishes.Where(d => d.Id == dishId).FirstOrDefaultAsync(ct);
    }
    public async Task<List<Dish>> GetDishesByIdsAsync(IEnumerable<int> dishesIds, CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        return await db.Dishes.Where(d => dishesIds.Contains(d.Id)).ToListAsync(ct);
    }
    public async Task<List<Dish>> GetDishesByNameAsync(string searchedName, CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        return await db.Dishes.Where(d => d.Name.ToLower().Contains(searchedName.ToLower())).ToListAsync(ct);
    }
    public async Task<List<Dish>> GetEnabledDishesAsync(CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        return await db.Dishes.Where(d => d.Enabled == true).ToListAsync(ct);
    }
    public async Task<List<Dish>> GetDisabledDishesAsync(CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        return await db.Dishes.Where(d => d.Enabled == false).ToListAsync(ct);
    }
    public async Task<Dish> ToggleDishEnabledAsync(int dishId, CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        var dish = await db.Dishes.Where(d => d.Id == dishId).FirstOrDefaultAsync(ct) ?? throw new DishNotFoundException(dishId);
        dish.Enabled = !dish.Enabled;
        await db.SaveChangesAsync(ct);
        Log.Information("Dish {DishId} is now {Enabled}", dish.Id, dish.Enabled);
        return dish;
    }

    // Customers
    public async Task<List<Customer>> GetAllCustomersAsync(CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        return await db.Customers.ToListAsync(ct);
    }
    public async Task<Customer?> GetCustomerByIdAsync(int customerId, CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        return await db.Customers.Where(c => c.Id == customerId).FirstOrDefaultAsync(ct);
    }
    public async Task<List<Customer>> GetCustomersByIdsAsync(IEnumerable<int> customersIds, CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        return await db.Customers.Where(c => customersIds.Contains(c.Id)).ToListAsync(ct);
    }
    public async Task<List<Customer>> GetCustomerByNameAsync(string searchedName, CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        return await db.Customers.Where(c => c.Name.ToLower().Contains(searchedName.ToLower())).ToListAsync(ct);
    }
    public async Task<Customer> AddCustomerAsync(Customer customer, CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        await db.Customers.AddAsync(customer, ct);
        await db.SaveChangesAsync(ct);
        Log.Information("Added Customer {name} with id {id}", customer.Name, customer.Id);
        return customer;
    }
    public async Task DeleteCustomerByIdAsync(int customerId, CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);
        
        var customer = await db.Customers.Where(c => c.Id == customerId).FirstOrDefaultAsync(ct) ?? throw new CustomerNotFoundException(customerId);

        db.Customers.Remove(customer);
        await db.SaveChangesAsync(ct);
        Log.Information("Customer with id {id} was removed", customerId);
    }
}