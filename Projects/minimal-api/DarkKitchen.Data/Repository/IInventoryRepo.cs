using DarkKitchen.Data.Entities;

namespace DarkKitchen.Data.Repository;

public interface IInventoryRepo
{
    Task<Dictionary<int, decimal>> GetCurrentStocksAsync(IEnumerable<int> ingredientIds, CancellationToken ct);
    Task<List<Ingredient>> GetIngredientsByIdsAsync(IEnumerable<int> ingredientIds, CancellationToken ct);
    Task UpdateIngredientsAsync(IEnumerable<Ingredient> ingredients, CancellationToken ct);
}