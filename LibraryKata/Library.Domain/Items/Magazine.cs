namespace LibraryKata.Domain;

public sealed class Magazine : LibraryItem, ILendable
{
    public int CirculationCopies { get; private set; }
    public Magazine(string title, string publisher, int circulationCopies) : base(title, publisher)
    {
        CirculationCopies = circulationCopies;
    }

    public override string Describe()
    {
        return $"{Title} magazine, published by {Author}";
    }

    public new string ShelfLabel()
    {
        return $"MAG-{Id} {Title}";
    }

    public bool Checkout()
    {
        if(CirculationCopies == 0)
        {
            return false;
        } 
        CirculationCopies--;
        return true;

    }

    public void Return()
    {
        CirculationCopies++;
    }
}