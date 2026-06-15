namespace LibraryKata.Domain;

public class Catalog
{
    public List<LibraryItem> _items = [];
    public int Count => _items.Count;
    public readonly Stack<LibraryItem> _returnCart = [];
    public readonly Queue<LibraryItem> _holdQueue = [];
    public readonly LinkedList<LibraryItem> _readingList = [];
}