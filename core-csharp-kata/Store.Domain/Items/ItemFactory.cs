namespace Store.Domain;

public static class ItemFactory
{
    private static int _nextId = 1;
    public static int getNextId => _nextId;
    public static Item Create(
        ItemKind kind,
        // int Id,
        // string Category,

        // General Attributes 
        string name, 
        double price, 
        int stock,
        // Specific Attributes of Clothing
        string size = "", 
        string color = "", 
        string material = "",
        // Specific Attributes of Electronics
        int warrantyYears = 0, 
        int powerConsumption = 0,
        // Specific Attributes of Grocery
        DateOnly expirationDate = new DateOnly(), 
        double weightKg = 0,
        // Specific Attibutes of Pokemon
        int pokeId = 1,
        string pokeType = "normal"
    )
    {
        switch (kind)
        {
            case ItemKind.Clothing:
                return new Clothing(
                    _nextId++,
                    name,
                    price,
                    stock,
                    size,
                    color,
                    material
                );
            case ItemKind.Electronics:
                return new Electronic(
                    _nextId++,
                    name,
                    price,
                    stock,
                    warrantyYears,
                    powerConsumption
                );
            case ItemKind.Grocery:
                return new Grocery(
                    _nextId++,
                    name,
                    price,
                    stock,
                    expirationDate,
                    weightKg
                );
            case ItemKind.Pokemon:
                return new Pokemon(
                    _nextId++,
                    name,
                    price,
                    stock,
                    pokeId,
                    pokeType
                );
        }

        throw new ArgumentException($"Unsupported ItemKind: {kind}", nameof(kind));
    }

}