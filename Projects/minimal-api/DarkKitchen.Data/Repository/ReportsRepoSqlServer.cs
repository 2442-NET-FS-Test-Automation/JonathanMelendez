using Microsoft.EntityFrameworkCore;

using DarkKitchen.Data.DTOs;

namespace DarkKitchen.Data.Repository;

public class ReportsRepoSqlServer(IDbContextFactory<DarkKitchenDbContext> dbF) : IReportsRepo
{
    private readonly IDbContextFactory<DarkKitchenDbContext> _dbF = dbF;

    public async Task<List<RankingDTO>> GetDishesRankedReport(CancellationToken ct)
    {
        await using var db = await _dbF.CreateDbContextAsync(ct);

        var res = await db.FulfillmentEvents
            .Where(f => f.Result == FulfillmentResult.Fulfilled)
            .Join(db.OrderLines, e => e.OrderId, l => l.OrderId, (e, l) => l)
            .GroupBy(l => l.DishId)
            .Select(g => new { DishId = g.Key, Units = g.Sum(l => l.Quantity) })
            .Join(db.Dishes, g => g.DishId, d => d.Id, (g, d) => new { d.Name, UnitsSold = g.Units })
            .OrderByDescending(x => x.UnitsSold)
            .ToListAsync(ct);

        return res.Select(r => new RankingDTO(r.Name, r.UnitsSold)).ToList();
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