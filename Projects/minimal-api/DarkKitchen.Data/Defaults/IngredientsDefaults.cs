namespace DarkKitchen.Data.Defaults;
public static class IngredientDefaults
{
    public static readonly Dictionary<int, decimal> InitialStocks = new()
    {
        { 1, 5 },   // White Rice
        { 2, 20 },  // Water
        { 3, 1 },   // Salt
        { 4, 5 },   // Milk
        { 5, 1 },   // Butter
        { 6, 2 },   // Sugar
        { 7, 50 },  // Condensed Milk
        { 8, 40 },  // Egg
        { 9, 5 },   // Flour
        { 10, 5 },  // Chicken
        { 11, 8 },  // Cheese
        { 12, 6 },  // Potato
        { 13, 15 }, // Tomato
        { 14, 3 },  // Ground Beef
        { 15, 10 }, // Onion
        { 16, 5 },  // Cream
        { 17, 3 }   // Pasta
    };
}