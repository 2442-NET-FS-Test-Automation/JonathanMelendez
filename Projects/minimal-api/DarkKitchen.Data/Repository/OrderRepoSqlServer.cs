using Microsoft.EntityFrameworkCore;
using Serilog;

using DarkKitchen.Data.Entities;
using DarkKitchen.Data.Exceptions;

namespace DarkKitchen.Data.Repository;

public class OrderRepoSqlServer(DarkKitchenDbContext db) : IOrderRepo
{
    private readonly DarkKitchenDbContext _db = db;
    
    // Orders
    public Task<Order?> GetOrderByIdAsync(int orderId, CancellationToken ct)
    {
        return _db.Orders
            .Include(o => o.Lines)
            .ThenInclude(l => l.Dish)
            .FirstOrDefaultAsync(o => o.Id == orderId, ct);
    }
    public async Task<List<Order>> GetOrdersByIdsAsync(IEnumerable<int> orderIds, CancellationToken ct)
    {
        return await _db.Orders
            .Where(o => orderIds.Contains(o.Id))
            .ToListAsync(ct);
    }
    public async Task<List<Order>> GetOrdersForCustomerAsync(int customerId, CancellationToken ct)
    {
        return await _db.Orders
            .Where(o => o.CustomerId == customerId)
            .Include(o => o.Lines)
            .ThenInclude(l => l.Dish)
            .ToListAsync(ct);
    }
    public async Task<List<Order>> GetAllOrdersAsync(CancellationToken ct)
    {
        return await _db.Orders
            .Include(o => o.Lines)
            .ThenInclude(l => l.Dish)
            .ToListAsync(ct);
    }
    public async Task AddOrderAsync(Order order, CancellationToken ct)
    {
        await _db.Orders.AddAsync(order, ct);
        await _db.SaveChangesAsync(ct);
        Log.Information("Added order with id {id}", order.Id);
    }
    public async Task AddOrdersAsync(IEnumerable<Order> orders, CancellationToken ct)
    {
        await _db.Orders.AddRangeAsync(orders, ct);
        await _db.SaveChangesAsync(ct);
        Log.Information("Added {qty} orders", orders.Count());
    }

    // Dishes
    public async Task<List<Dish>> GetAllDishesAsync(CancellationToken ct)
    {
        return await _db.Dishes.ToListAsync(ct);
    }
    public async Task<Dish?> GetDishByIdAsync(int dishId, CancellationToken ct)
    {
        return await _db.Dishes.Where(d => d.Id == dishId).FirstOrDefaultAsync(ct);
    }
    public async Task<List<Dish>> GetDishesByNameAsync(string searchedName, CancellationToken ct)
    {
        return await _db.Dishes.Where(d => d.Name.ToLower().Contains(searchedName.ToLower())).ToListAsync(ct);
    }
    public async Task<List<Dish>> GetEnabledDishesAsync(CancellationToken ct)
    {
        return await _db.Dishes.Where(d => d.Enabled == true).ToListAsync(ct);
    }
    public async Task<List<Dish>> GetDisabledDishesAsync(CancellationToken ct)
    {
        return await _db.Dishes.Where(d => d.Enabled == false).ToListAsync(ct);
    }
    public async Task<Dish> ToggleDishEnabledAsync(int dishId, CancellationToken ct)
    {
        var dish = await _db.Dishes.Where(d => d.Id == dishId).FirstOrDefaultAsync(ct) ?? throw new DishNotFoundException(dishId);
        dish.Enabled = !dish.Enabled;
        await _db.SaveChangesAsync(ct);
        Log.Information("Dish {DishId} is now {Enabled}", dish.Id, dish.Enabled);
        return dish;
    }
}