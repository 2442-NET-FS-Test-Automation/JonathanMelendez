namespace LibraryKata.Domain;

public class Shelf<T>
{
    private readonly T[] _slots;
    private int _used = 0;
    public Shelf(int capacity)
    {
        _slots = new T[capacity];
    }

    public int Capacity => _slots.Length;
    public int Count => _used;
    public bool TryAdd(T item)
    {
        if (_used == Capacity)
        {
            return false;
        }
        _slots[_used++] = item;
        return true;
    }
    public T Get(int index)
    {
        return _slots[index];
    }
}