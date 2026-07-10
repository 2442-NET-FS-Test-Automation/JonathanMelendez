using DarkKitchen.Data.Entities;

namespace DarkKitchen.Data.Repository;

public class InventoryRepoSqlServer(DarkKitchenDbContext db) : IInventoryRepo
{
    private readonly DarkKitchenDbContext _db = db;
    public Task<Dictionary<int, decimal>> GetCurrentStocksAsync(IEnumerable<int> ingredientIds, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<List<Ingredient>> GetIngredientsByIdsAsync(IEnumerable<int> ingredientIds, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task UpdateIngredientsAsync(IEnumerable<Ingredient> ingredients, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}