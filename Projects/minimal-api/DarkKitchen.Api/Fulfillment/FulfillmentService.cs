using Microsoft.EntityFrameworkCore;
using Serilog;

using DarkKitchen.Data;
using DarkKitchen.Data.Entities;
using DarkKitchen.Data.Repository;


namespace DarkKitchen.Api.Fulfillment;

public interface IFulfillmentService
{
    public Task<FulfillmentResult> FulfillOneAsync(int orderId, CancellationToken ct);
    public Task<BurstResult> FulfillBurstAsync(IEnumerable<int> orderIds, CancellationToken ct);
}

public record BurstResult(int Fulfilled, int Backordered);
public class FulfillmentService(
    IDbContextFactory<DarkKitchenDbContext> dbF,
    IDarkKitchenRepo repo
) : IFulfillmentService
{
    private readonly IDarkKitchenRepo _repo = repo;
    private readonly IDbContextFactory<DarkKitchenDbContext> _dbF = dbF;
    public async Task<FulfillmentResult> FulfillOneAsync(int orderId, CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        // Get order from db
        var order = await _repo.GetOrderByIdAsync(orderId, ct) ?? throw new Exception();

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

        if (await SaveWithRetryAsync(db, ingredientsRequired, ct))
        {
            order.Status = OrderStatus.Fulfilled;
            db.FulfillmentEvents.Add(new FulfillmentEvent { OrderId = orderId, Result = FulfillmentResult.Fulfilled});
        
            await db.SaveChangesAsync(ct);
            Log.Information("Fulfilled {OrderId}.", orderId);   
            return FulfillmentResult.Fulfilled;
        }
        db.ChangeTracker.Clear();

        var freshOrder = await _repo.GetOrderByIdAsync(orderId, ct) ?? throw new Exception();

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
            orders = await _repo.GetOrdersByIdsAsync(idList, ct);
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
        var ingredientIds = ingredientsRequired.Keys.ToList();
        // Get the actual entities
        var ingredientsToUpdate = await db.Ingredients
            .Where(i => ingredientIds.Contains(i.Id))
            .ToListAsync(ct);
        
        foreach (var ingredient in ingredientsToUpdate)
        {
            if (ingredient.Stock >= ingredientsRequired[ingredient.Id])
                ingredient.Stock -= ingredientsRequired[ingredient.Id];
            else return false;
        }
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
            ct.ThrowIfCancellationRequested();
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