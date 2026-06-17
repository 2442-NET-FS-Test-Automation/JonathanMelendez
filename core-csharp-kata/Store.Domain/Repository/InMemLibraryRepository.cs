using Serilog;

namespace Store.Domain;

public class InMemLibraryRepository() : ILibraryRepository
{
    private Dictionary<int, Item> _items = [];
    public void AddItem(Item item)
    {
        _items.Add(item.Id, item); // This one throws when duplicates

        Log.Information("Added {Name} with id {id}", item.Name, item.Id);
    }

    public List<Item> GetAllItems() => _items.Values.ToList();

    public Item GetItemById(int id)
    {
        if (_items.TryGetValue(id, out Item? item)) return item;

        Log.Warning("Lookup failed for {id}", id);
        // throw new ItemNotFoundException(id);
        throw new Exception(); //  TODO: ===================================================================
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
}