using Serilog;
namespace LibraryKata.Domain;

public class InMemLibraryRepo() : ILibraryRepo
{
    private Dictionary<int, LibraryItem> _items = [];
    public void AddItem(LibraryItem item)
    {
        _items.Add(item.Id, item); // This one throws when duplicates
        // _items[item.Id] = item; // Alternative for adding/assigning to dictionaries

        Log.Information("Added {Title} - id: {id}", item.Title, item.Id);
    }

    public List<LibraryItem> GetAllItems() => _items.Values.ToList();

    public LibraryItem GetItemById(int id)
    {
        if (_items.TryGetValue(id, out LibraryItem? item)) return item;

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
}