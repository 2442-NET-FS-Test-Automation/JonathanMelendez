using Library.ControllerApi.DTOs;
using Library.Data;
using Library.Data.Entities;

namespace Library.ControllerApi.Services;

public class InventoryService(IInventoryRepository repo)
    : IInventoryService
{
    private readonly IInventoryRepository _repo = repo;

    public Task<IReadOnlyList<InventoryItem>> GetAllAsync() => _repo.GetAllAsync();
    public Task<InventoryItem?> GetBySkuAsync(string sku) => _repo.GetInventoryItemBySkuAsync(sku);
    public Task<InventoryItem> AddAsync(InventoryDTO dto) => _repo.AddInventoryItemAsync(dto.Sku, dto.Name, dto.Price, dto.CurrentStock);
    public Task<bool> RemoveAsync(string sku) => _repo.RemoveBySkuAsync(sku);
}