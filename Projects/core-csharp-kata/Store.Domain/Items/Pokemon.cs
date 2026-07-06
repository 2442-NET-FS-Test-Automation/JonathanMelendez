namespace Store.Domain;

public class Pokemon : Item
{
    public override string Category => "Pokemon";
    public int PokeId { get; }
    public string PokeType { get; }
    
    public Pokemon(int id, string name, double price, int stock, int pokeId, string pokeType) : base(id, name, price, stock)
    {
        PokeId = pokeId;
        PokeType = pokeType;
    }
    
    public override string GetDetails()
    {
        return $"PokeId {PokeId}, PokeType {PokeType}";
    }

    public override Dictionary<string, string> Describe()
    {
        return new Dictionary<string, string>
        {
            ["ID"]          = $"{Id}",
            ["Name"]        = $"{Name}",
            ["Stock"]       = $"{Stock}",
            ["Price"]       = $"{Price}",
            ["PokeId"]      = $"{PokeId}",
            ["PokeType"]    = $"{PokeType}"
        };
    }
}