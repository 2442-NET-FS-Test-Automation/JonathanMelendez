using Library.ControllerApi.DTOs;
using Library.Data.Entities;

namespace Library.ControllerApi.Services;

public interface IInventoryService
{
    Task<IReadOnlyList<InventoryItem>> GetAllAsync();
    Task<InventoryItem?> GetBySkuAsync(string sku);
    Task<InventoryItem> AddAsync(InventoryDTO newItem);
    Task<bool> RemoveAsync(string sku);
}