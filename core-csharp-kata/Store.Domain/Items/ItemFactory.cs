namespace Store.Domain;

public static class ItemFactory
{
    public static Item Create(
        ItemKind kind,
        // int Id,
        // string Category,

        // General Attributes 
        string name, 
        double price, 
        int stock,
        // Specific Attributes of Clothing
        string size, 
        string color, 
        string material,
        // Specific Attributes of Electronics
        int warrantyYears, 
        int powerConsumption,
        // Specific Attributes of Grocery
        DateOnly expirationDate, 
        double weightKg
    )
    {
        switch (kind)
        {
            case ItemKind.Clothing:
                return new Clothing(
                    name,
                    price,
                    stock,
                    size,
                    color,
                    material
                );
            case ItemKind.Electronics:
                return new Electronic(
                    name,
                    price,
                    stock,
                    warrantyYears,
                    powerConsumption
                );
            case ItemKind.Grocery:
                return new Grocery(
                    name,
                    price,
                    stock,
                    expirationDate,
                    weightKg
                );
        }

        throw new ArgumentException($"Unsupported ItemKind: {kind}", nameof(kind));
    }

}