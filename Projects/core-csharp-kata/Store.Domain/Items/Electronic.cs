namespace Store.Domain;

public class Electronic : Item
{
    public override string Category => "Electronic";
    public int WarrantyYears { get; }
    public int PowerConsumption { get; }
    public Electronic(int id, string name, double price, int stock, int warrantyYears, int powerConsumption) : base(id, name, price, stock)
    {
        WarrantyYears = warrantyYears;
        PowerConsumption = powerConsumption;
    }
    public override string GetDetails()
    {
        return $"{WarrantyYears} years of warranty, {PowerConsumption}W of consumption";
    }
    public override Dictionary<string, string> Describe()
    {
        return new Dictionary<string, string>
        {
            ["ID"] = $"{Id}",
            ["Name"] = $"{Name}",
            ["Stock"] = $"{Stock}",
            ["Price"] = $"{Price}",
            ["Years of warranty"] = $"{WarrantyYears}",
            ["Power Consumption"] = $"{PowerConsumption}w",
        };
    }
}