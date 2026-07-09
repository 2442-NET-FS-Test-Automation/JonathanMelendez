using Microsoft.EntityFrameworkCore;
using Serilog;

using DarkKitchen.Data;
using DarkKitchen.Data.Entities;

namespace DarkKitchen.Api.Fulfillment;

public interface IFulfillmentService
{
    public Task<FulfillmentResult> FulfillOneAsync(int orderId, CancellationToken ct);
    public Task<BurstResult> FulfillBurstAsync(IEnumerable<int> orderIds, CancellationToken ct);
}

public record BurstResult(int Fulfilled, int Backordered);
public class FulfillmentService(
    IDbContextFactory<DarkKitchenDbContext> dbF
) : IFulfillmentService
{
    private readonly IDbContextFactory<DarkKitchenDbContext> _dbF = dbF;
    public async Task<FulfillmentResult> FulfillOneAsync(int orderId, CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        // Get order from db
        var order = await db.Orders
            .Include(o => o.Lines)
                .ThenInclude(o => o.Dish)
                    .ThenInclude(d => d.Ingredients)
            .FirstAsync(o => o.Id == orderId, ct);

        if (order.Status != OrderStatus.Pending)
        {
            Log.Warning("Order {OrderId} is not pending, cannot fulfill", orderId);
            return order.Status == OrderStatus.Fulfilled ? FulfillmentResult.Fulfilled : FulfillmentResult.Backordered;
        }

        // Calculate total ingredients required
        var ingredientsRequired = order.Lines
            .SelectMany(line => line.Dish.Ingredients.Select(di => new 
                { di.IngredientId, AmountNeeded = di.Quantity * line.Quantity }))
            .GroupBy(x => x.IngredientId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.AmountNeeded));

        // Get current Stock from db
        var ingredientIds = ingredientsRequired.Keys.ToList();
        var currentStocks = await db.Ingredients
            .Where(i => ingredientIds.Contains(i.Id))
            .Select(i => new { i.Id, i.Stock })
            .ToDictionaryAsync(i => i.Id, i => i.Stock, ct);

        // Check if we have all ingredients available
        bool canFulfill = ingredientsRequired.All(ing => currentStocks[ing.Key] >= ing.Value);

        if (canFulfill)
        {
            // Get the actual entities
            var ingredientsToUpdate = await db.Ingredients
                .Where(i => ingredientIds.Contains(i.Id))
                .ToListAsync(ct);
        
            foreach (var ingredient in ingredientsToUpdate)
            {
                ingredient.Stock -= ingredientsRequired[ingredient.Id];
            }
            order.Status = OrderStatus.Fulfilled;
            db.FulfillmentEvents.Add(new FulfillmentEvent { OrderId = orderId, Result = FulfillmentResult.Fulfilled});
                
            if (await SaveWithRetryAsync(db, ingredientsRequired, ct))
            {
                Log.Information("Fulfilled {OrderId}.", orderId);   
                return FulfillmentResult.Fulfilled;
            }
            // if save failed, clear all changes that could still be made to ingredients
            db.ChangeTracker.Clear();
        }

        // If theres not enough stock to begin with we get here        
        var freshOrder = await db.Orders.FirstAsync(o => o.Id == orderId, ct);

        freshOrder.Status = OrderStatus.Backordered;
        db.FulfillmentEvents.Add(new FulfillmentEvent { OrderId = orderId, Result = FulfillmentResult.Backordered});
        
        Log.Warning("Backordered {OrderId}: insufficient stock", orderId);

        await db.SaveChangesAsync(ct);
        return FulfillmentResult.Backordered;
    }
    public async Task<BurstResult> FulfillBurstAsync(IEnumerable<int> orderIds, CancellationToken ct)
    {
        List<int> idList = orderIds.ToList();
        List<Order> orders;

        await using (var db = await _dbF.CreateDbContextAsync(ct))
        {   // Look in our db, grab every order that appears in our idList
            orders = await db.Orders.Where(o => idList.Contains(o.Id)).ToListAsync(ct);
        }
        
        var planned = OrderByPriority(orders);
        var tasks = planned.Select(id => FulfillOneAsync(id, ct));

        // Await here until all tasks are complete
        var results = await Task.WhenAll(tasks);

        return new BurstResult(
            Fulfilled: results.Count(r => r == FulfillmentResult.Fulfilled),
            Backordered: results.Count(r => r == FulfillmentResult.Backordered)
        );
    }
    public async Task<bool> SaveWithRetryAsync(
        DarkKitchenDbContext db, 
        IReadOnlyDictionary<int, decimal> ingredientsRequired, CancellationToken ct
    )
    {
        while (true) { // Keep trying until the stock gets dried or some other thing fail
            try
            {
                await db.SaveChangesAsync(ct);
                return true;
            }
            catch (DbUpdateConcurrencyException e)
            {
                Log.Warning("Attempting save retry");
                foreach (var entry in e.Entries)
                {
                    var current = await entry.GetDatabaseValuesAsync(ct);
                    if (current is null) return false;

                    entry.OriginalValues.SetValues(current);    
                    if (entry.Entity is Ingredient ing)
                    {
                        // Grab the current totals
                        decimal freshValue = current.GetValue<decimal>(nameof(Ingredient.Stock));
                        decimal desiredAmount = ingredientsRequired[ing.Id];

                        // Re-check on the fresh stock
                        if (freshValue < desiredAmount) return false;
                        ing.Stock = freshValue - desiredAmount;
                    }
                }
            }
        }
    }

    public IReadOnlyList<int> OrderByPriority(IEnumerable<Order> orders)
    {
        PriorityQueue<int, int> pq = new();

        foreach (Order o in orders)
            pq.Enqueue(o.Id, o.Priority == OrderPriority.Urgent ? 0 : 1);

        var orderedByPriority = new List<int>();

        while (pq.TryDequeue(out int id, out _))
        {
            orderedByPriority.Add(id);
        }

        return orderedByPriority;
    }
}