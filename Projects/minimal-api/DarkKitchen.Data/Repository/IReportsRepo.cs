using DarkKitchen.Data.DTOs;

namespace DarkKitchen.Data.Repository;

public interface IReportsRepo
{
    Task<List<RankingDTO>> GetDishesRankedReport(CancellationToken ct);
    Task<List<RankingDTO>> GetCustomersRankedReport(CancellationToken ct);
    Task<List<FulfillmentRateDTO>> GetFulfillmentRateReport(CancellationToken ct);
}