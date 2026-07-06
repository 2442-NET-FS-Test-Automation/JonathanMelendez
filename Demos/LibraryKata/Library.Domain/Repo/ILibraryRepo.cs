namespace LibraryKata.Domain;

public interface ILibraryRepo
{
    // CRUD
    void AddItem(LibraryItem item);
    void AddItem(LibraryItem[] items)
    {
        foreach (LibraryItem item in items) AddItem(item);
    }
    void AddItem(List<LibraryItem> items) => AddItem(items.ToArray());

    LibraryItem GetItemById(int id); // Trows ItemNotFoundException if not found
    List<LibraryItem> GetAllItems();

    bool RemoveById(int id);
}