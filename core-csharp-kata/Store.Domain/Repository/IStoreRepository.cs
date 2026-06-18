namespace Store.Domain;
public interface IStoreRepository
{
    // CRUD
    void AddItem(Item item);
    void AddItem(Item[] items)
    {
        foreach (Item item in items) AddItem(item);
    }
    void AddItem(List<Item> items) => AddItem(items.ToArray());
    public Item GetLastItem();
    Item GetItemById(int id); // Throws ItemNotFoundException if not found
    List<Item> GetAllItems();

    bool RemoveById(int id);
}