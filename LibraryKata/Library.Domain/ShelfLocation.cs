namespace LibraryKata.Domain;

public struct ShelfLocation(int aisle, int shelf)
{
    public int Aisle { get; } = aisle;
    public int Shelf { get; } = shelf;

    public override readonly string ToString() => $"Aisle {Aisle}, Shelf {Shelf}";
}