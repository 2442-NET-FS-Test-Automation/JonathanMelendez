namespace Store.Domain;
public interface IStoreRepository
{
    public enum OrderType
    {
        ById,
        ByName,
        ByPrice,
        ByStock,
        ByCategory
    }
    // CRUD
    public void AddItem(Item item);
    public void AddItem(Item[] items)
    {
        foreach (Item item in items) AddItem(item);
    }
    public void AddItem(List<Item> items) => AddItem(items.ToArray());
    public Item GetLastItem();
    Item GetItemById(int id); // Throws ItemNotFoundException if not found
    List<Item> GetAllItems();
    public IEnumerable<Item> Find(Predicate<Item> match);

    public bool RemoveById(int id);

    public List<Item> OrderList( OrderType match);
}