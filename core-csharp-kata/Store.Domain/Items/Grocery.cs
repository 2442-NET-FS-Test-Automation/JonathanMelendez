namespace Store.Domain;

public class Grocery : Item
{
    public override string Category => "Groceries";
    public DateOnly ExpirationDate { get; }
    public double WeightKg { get; }
    public Grocery(int id, string name, double price, int stock, DateOnly expirationDate, double weightKg) : base( id, name, price, stock)
    {
        ExpirationDate = expirationDate;
        WeightKg = weightKg;
    }
    public override string GetDetails()
    {
        return $"Expires on {ExpirationDate}, Weight {WeightKg}Kg";
    }
    public override Dictionary<string, string> Describe()
    {
        return new Dictionary<string, string>
        {
            ["ID"] = $"{Id}",
            ["Stock"] = $"{Stock}",
            ["Price"] = $"{Price}",
            ["Expiration Date"] = $"{ExpirationDate}",
            ["Weight"] = $"{WeightKg} Kg",
        };
    }
}