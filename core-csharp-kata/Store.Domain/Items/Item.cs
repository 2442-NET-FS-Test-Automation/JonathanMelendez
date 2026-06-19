namespace Store.Domain;

public abstract class Item(string name, double price, int stock)
{
    public static int _nextId = 1;
    public int Id { get; } = _nextId++;
    public string Name { get; private set; } = name;
    public double Price { get; private set; } = price;
    public int Stock { get; private set; } = stock;
    public virtual string? Category { get; }

    public bool Sell(int amount)
    {
        if (Stock < amount) return false;
        Stock -= amount;
        return true;
    }
    public void Restock(int amount) => Stock += amount;
    public override string ToString()
    {
        return $"[{Id}] {Name} - ${Price:F2}: {Stock} available";
    }
    public abstract string GetDetails();

    public abstract Dictionary<string, string> Describe();
}
