using System.Collections;

namespace LibraryKata.Domain;

public partial class Catalog : IEnumerable<LibraryItem>
{

    public IEnumerator<LibraryItem> GetEnumerator()
    {
        foreach (LibraryItem item in _items)
        {
            // Lazily return items one by one
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerable<LibraryItem> GetLendableItems()
    {
        foreach (LibraryItem item in _items)
        {
            if (item is ILendable)
            {
                yield return item;
            }
        }
    }
    public List<LibraryItem> Find(Predicate<LibraryItem> match)
    {
        List<LibraryItem> foundItems = [];

        foreach (LibraryItem item in _items)
        {
            if (match(item))
            {
                foundItems.Add(item);
            }
        }
        return foundItems;
    }
}