using Library.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Data;

public class InventoryRepository(IDbContextFactory<LibraryDbContext> factory) 
    : IInventoryRepository
{
    private readonly IDbContextFactory<LibraryDbContext> _factory = factory;

    public async Task<IReadOnlyList<InventoryItem>> GetAllAsync(CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        return await db.Inventory.Include(i => i.Product).ToListAsync(ct);
    }

    public async Task<InventoryItem?> GetInventoryItemBySkuAsync(string sku, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        return await db.Inventory.Include(i => i.Product).FirstOrDefaultAsync(i => i.Product.Sku == sku);
    }

    public async Task<InventoryItem> AddInventoryItemAsync(string sku, string name, decimal price, int quantity)
    {
        await using var db = await _factory.CreateDbContextAsync();

        InventoryItem newItem = new InventoryItem
        {
            Product = new Product { Sku = sku, Name = name, Price = price },
            CurrentStock = quantity
        };

        db.Inventory.Add(newItem);
        await db.SaveChangesAsync();

        return newItem;
    }

    public async Task<bool> RemoveBySkuAsync(string sku)
    {
        await using var db = await _factory.CreateDbContextAsync();

        InventoryItem? itemToRemove = await db.Inventory.Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.Product.Sku == sku);
        if (itemToRemove is null)
        {
            return false;
        }

        db.Products.Remove(itemToRemove.Product);
        await db.SaveChangesAsync();

        return true;
    }
}