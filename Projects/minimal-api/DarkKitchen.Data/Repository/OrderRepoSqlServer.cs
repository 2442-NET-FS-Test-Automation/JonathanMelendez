using DarkKitchen.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Data.Repository;

public class OrderRepoSqlServer(DarkKitchenDbContext db) : IOrderRepo
{
    private readonly DarkKitchenDbContext _db = db;
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
    }
    public async Task AddOrdersAsync(IEnumerable<Order> orders, CancellationToken ct)
    {
        await _db.Orders.AddRangeAsync(orders, ct);
        await _db.SaveChangesAsync(ct);
    }
}