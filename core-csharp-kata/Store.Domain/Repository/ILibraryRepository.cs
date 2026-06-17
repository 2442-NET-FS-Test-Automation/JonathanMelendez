namespace Store.Domain;
public interface ILibraryRepository
{
    // CRUD
    void AddItem(Item item);
    void AddItem(Item[] items)
    {
        foreach (Item item in items) AddItem(item);
    }
    void AddItem(List<Item> items) => AddItem(items.ToArray());

    Item GetItemById(int id); // Trows ItemNotFoundException if not found
    List<Item> GetAllItems();

    bool RemoveById(int id);
}