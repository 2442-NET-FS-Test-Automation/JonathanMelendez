namespace Store.Domain;

public class Clothing : Item
{
    public override string Category => "Clothing";
    public string Size { get; }
    public string Color { get; }
    public string Material { get; }
    public Clothing(int id, string name, double price, int stock, string size, string color, string material) : base(id, name, price, stock)
    {
        Size = size;
        Color = color;
        Material = material;
    }
    public override string GetDetails()
    {
        return $"Size {Size}, Color {Color}, Material {Material}";
    }

    public override Dictionary<string, string> Describe()
    {
        return new Dictionary<string, string>
        {
            ["ID"] = $"{Id}",
            ["Stock"] = $"{Stock}",
            ["Price"] = $"{Price}",
            ["Size"] = $"{Size}",
            ["Color"] = $"{Color}",
            ["Material"] = $"{Material}"
        };
    }
}