namespace LibraryKata.Domain;
public readonly struct ShelfLocation
{
    public int Aisle { get; }
    public int Shelf { get; }

    public ShelfLocation(int aisle, int shelf)
    {
        Aisle = aisle;
        Shelf = shelf;
    }

    public override string ToString()
    {
        return $"Aisle {Aisle}, Shelf {Shelf}";
    }
}