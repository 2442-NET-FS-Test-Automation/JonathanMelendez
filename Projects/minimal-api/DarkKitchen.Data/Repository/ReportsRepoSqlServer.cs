using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

using DarkKitchen.Data.DTOs;

namespace DarkKitchen.Data.Repository;

public class ReportsRepoSqlServer(IDbContextFactory<DarkKitchenDbContext> dbF) : IReportsRepo
{
    private readonly IDbContextFactory<DarkKitchenDbContext> _dbF = dbF;
    private static readonly ConcurrentDictionary<int, string> _dishNameCache = new();

    public async Task<List<RankingDTO>> GetDishesRankedReport(CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        // First, try to get dish names from cache
        var dishIds = await db.Dishes.Select(d => d.Id).ToListAsync(ct);
        foreach (var id in dishIds)
        {
            if (!_dishNameCache.ContainsKey(id))
            {
                var name = await db.Dishes.Where(d => d.Id == id).Select(d => d.Name).FirstOrDefaultAsync(ct);
                _dishNameCache.TryAdd(id, name!);
            }
        }

        // Now build the report using the cache
        var res = await db.FulfillmentEvents
            .Where(f => f.Result == FulfillmentResult.Fulfilled)
            .Join(db.OrderLines, e => e.OrderId, l => l.OrderId, (e, l) => l)
            .GroupBy(l => l.DishId)
            .Select(g => new { DishId = g.Key, Units = g.Sum(l => l.Quantity) })
            .OrderByDescending(x => x.Units)
            .ToListAsync(ct);

        // Convert to DTO using cached names
        return res.Select(r => new RankingDTO(
            _dishNameCache.GetOrAdd(r.DishId, id => db.Dishes.Where(d => d.Id == id).Select(d => d.Name).FirstOrDefault()!),
            r.Units
        )).ToList();
    }
    public async Task<List<RankingDTO>> GetCustomersRankedReport(CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        var res = db.FulfillmentEvents
            .Where(f => f.Result == FulfillmentResult.Fulfilled)
            .Join(db.Orders, e => e.OrderId, o => o.Id, (e, o) => o)
            .GroupBy(o => o.CustomerId)
            .Select(g => new { CustomerId = g.Key, Orders = g.Count() })
            .Join(db.Customers, g => g.CustomerId, c => c.Id, (g, c) => new { c.Name, g.Orders })
            .OrderByDescending(x => x.Orders);
        return res.Select(r => new RankingDTO(r.Name, r.Orders)).ToList();
    }
    public async Task<List<FulfillmentRateDTO>> GetFulfillmentRateReport(CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        var res = await db.FulfillmentEvents
            .GroupBy(g => g.Result)
            .Select(g => new { Result = g.Key.ToString(), Quantity = g.Count() })
            .ToListAsync(ct);

        return res.Select(r => new FulfillmentRateDTO(r.Result, r.Quantity)).ToList();
    }
}