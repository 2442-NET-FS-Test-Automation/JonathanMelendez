using Library.Data;
using Library.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Library.Api.Fulfillment;

public interface IFulfillmentService
{
    public Task<FulfillmentResult> FulfillOneAsync(int orderId, CancellationToken ct);
    public Task<BurstResult> FulfillBurstAsync(IEnumerable<int> orderIds, CancellationToken ct);
}

public enum FulfillmentResult { Fulfilled, Backordered }

// record has the same idea as a struct but they are reference types
public record BurstResult(int Fulfilled, int Backordered);
public class FulfillmentService : IFulfillmentService
{
    private readonly IDbContextFactory<LibraryDbContext> _factory;
    private readonly BurstPlanner _planner;
    public FulfillmentService(IDbContextFactory<LibraryDbContext> factory, BurstPlanner planner)
    {
        _factory = factory;
        _planner = planner;
    }
    public async Task<FulfillmentResult> FulfillOneAsync(int orderId, CancellationToken ct)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);

        var order = await db.Orders.Include(o => o.Lines).FirstAsync(o => o.Id == orderId, ct);

        var requested = order.Lines.ToDictionary(l => l.ProductId, l => l.Quantity);

        bool canFulfill = true;

        foreach (OrderLine line in order.Lines)
        {
            InventoryItem inv = await db.Inventory.FirstAsync(i => i.ProductId == line.ProductId, ct);
            
            if (inv.CurrentStock < line.Quantity)
            {
                canFulfill = false;
                break;
            }
            inv.CurrentStock -= line.Quantity;
        }
        if (!canFulfill)
        {
            order.Status = Status.Backordered;
            db.FulfillmentEvents.Add(new FulfillmentEvent { OrderId = orderId, Type = "Backordered" });  
            
            await db.SaveChangesAsync(ct);
            
            Log.Warning("Backordered {orderId}: insufficient stock", orderId);

            return FulfillmentResult.Backordered;
        }

        order.Status = Status.Fulfilled;
        order.CompletedUtc = DateTime.UtcNow;
        db.FulfillmentEvents.Add(new FulfillmentEvent {OrderId = orderId, Type = "Fulfilled" });

        if (!await SaveWithRetryAsync(db, requested, ct))
        {
            db.ChangeTracker.Clear();
            Order staleOrder = await db.Orders.FirstAsync( o => o.Id == orderId, ct);

            staleOrder.Status = Status.Backordered;
            Log.Warning("Backordered {orderId}", orderId);
            return FulfillmentResult.Backordered;
        }

        Log.Information("Fulfilled {orderId}, {LineCount} lines", orderId, order.Lines.Count);
        return FulfillmentResult.Fulfilled;
    }
    private static async Task<bool> SaveWithRetryAsync(
        LibraryDbContext db, 
        IReadOnlyDictionary<int, int> requestedByProductId, 
        CancellationToken ct
    )
    {
        while (true)
        {
            try
            {
                await db.SaveChangesAsync(ct);
                return true;
            }
            catch (DbUpdateConcurrencyException e)
            {
                foreach (var entry in e.Entries)
                {
                    var current = await entry.GetDatabaseValuesAsync();
                    
                    if (current is null) return false;

                    entry.OriginalValues.SetValues(current);
                    if (entry.Entity is InventoryItem inv)
                    {
                        int freshValue = current.GetValue<int>(nameof(InventoryItem.CurrentStock));
                        int desiredAmount = requestedByProductId[inv.ProductId];

                        if (freshValue < desiredAmount) return false;

                        inv.CurrentStock = freshValue - desiredAmount;
                    }
                }
            }
        }
    }

    public async Task<BurstResult> FulfillBurstAsync(IEnumerable<int> orderIds, CancellationToken ct)
    {
        List<int> idList = orderIds.ToList();

        List<Order> orders;
        await using (var db = await _factory.CreateDbContextAsync(ct))
        {
            orders = await db.Orders.Where(o => idList.Contains(o.Id)).ToListAsync(ct);
        }

        var planned = _planner.OrderByPriority(orders);
        
        var tasks = planned.Select(id => FulfillOneAsync(id, ct));

        var results = await Task.WhenAll(tasks);

        return new BurstResult(
            Fulfilled: results.Count(r => r == FulfillmentResult.Fulfilled),
            Backordered: results.Count(r => r == FulfillmentResult.Backordered)
        );
    }
}