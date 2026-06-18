using Serilog;

namespace Store.Domain;

public class InMemStoreRepository : IStoreRepository
{
    private static Dictionary<int, Item> _items =  GenSeedItems();
    public void AddItem(Item item)
    {
        _items.Add(item.Id, item); // This one throws when duplicates

        Log.Information("Added {Name} with id {id}", item.Name, item.Id);
    }
    public List<Item> GetAllItems() => _items.Values.ToList();
    public Item GetLastItem() => _items.Last().Value;
    public Item GetItemById(int id)
    {
        if (_items.TryGetValue(id, out Item? item)) return item;

        Log.Warning("Lookup failed for {id}", id);
        throw new ItemNotFoundException(id);
    }

    public bool RemoveById(int id)
    {
        if (_items.Remove(id))
        {
            Log.Information("Removed item with id {id}", id);
            return true;
        }
        
        Log.Information("Item with id {id} removed/not found", id);
        return false;
    }

    private static Dictionary<int, Item> GenSeedItems()
    {
        Dictionary<int, Item> genItems = [];
        genItems.Add(1, new Clothing("Shirt", 3, 10, "L", "White", "Poliester"));
        genItems.Add(2, new Clothing("Pants", 4.5, 5, "XL", "Gray", "Silk"));
        genItems.Add(3, new Electronic("Xbox Series Z", 500, 3, 2, 250));
        genItems.Add(4, new Electronic("Potato Station", 800, 7, 1, 300));
        genItems.Add(5, new Electronic("Televisor", 400, 12, 3, 50));
        genItems.Add(6, new Grocery("Doritos", 0.8, 25, new DateOnly(2026, 10, 17), 0.2));
        genItems.Add(7, new Grocery("Rice Bag", 0.8, 25, new DateOnly(2026, 10, 17), 1));
        return genItems;
    }
}