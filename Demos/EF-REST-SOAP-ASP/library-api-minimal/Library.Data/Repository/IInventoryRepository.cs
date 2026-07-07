using Library.Data.Entities;

namespace Library.Data;

public interface IInventoryRepository
{
    Task<IReadOnlyList<InventoryItem>> GetAllAsync(CancellationToken ct = default);
    Task<InventoryItem?> GetInventoryItemBySkuAsync(string sku, CancellationToken ct = default);
    Task<InventoryItem> AddInventoryItemAsync(string sku, string name, decimal price, int quantity);
    Task<bool> RemoveBySkuAsync(string sku);
}