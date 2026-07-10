namespace DarkKitchen.Data.Defaults;
public static class IngredientDefaults
{
    public static readonly Dictionary<int, decimal> InitialStocks = new()
    {
        { 1, 20 },   // White Rice
        { 2, 20 },  // Water
        { 3, 2 },   // Salt
        { 4, 25 },   // Milk
        { 5, 20 },   // Butter
        { 6, 20 },   // Sugar
        { 7, 75 },  // Condensed Milk
        { 8, 200 },  // Egg
        { 9, 15 },   // Flour
        { 10, 15 },  // Chicken
        { 11, 20 },  // Cheese
        { 12, 15 },  // Potato
        { 13, 50 }, // Tomato
        { 14, 20 },  // Ground Beef
        { 15, 50 }, // Onion
        { 16, 10 },  // Cream
        { 17, 10 }   // Pasta
    };
}