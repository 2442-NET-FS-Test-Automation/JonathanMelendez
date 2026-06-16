using Serilog;
namespace LibraryKata.Domain;

public class InMemLibraryRepo() : ILibraryRepo
{
    private List<LibraryItem> _items = [];
    public void AddItem(LibraryItem item)
    {
        _items.Add(item);
        Log.Information("Added {Title} - id: {id}", item.Title, item.Id);
    }

    public List<LibraryItem> GetAllItems() => _items.ToList();

    public LibraryItem GetItemById(int id)
    {
        foreach (LibraryItem item in _items)
        {
            if (item.Id == id) return item;
        }

        Log.Warning($"Lookup failes for {id}");
        throw new ItemNotFoundException(id);
    }

    public bool RemoveById(int id)
    {
        foreach (LibraryItem item in _items)
        {
            if (item.Id == id) 
            {
                _items.Remove(item);
                Log.Information($"Removed item with id: {id}");
                return true;
            }
        }
        Log.Information($"Item with id {id} removed/not found");
        return false;
    }
}