using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Data.Entities;

public class DishIngredient
{
    public int Id { get; set; }
    public int DishId { get; set; }
    public Dish Dish { get; set; } = default!;
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = default!; 

    [Precision(10,3)]
    public decimal Quantity { get; set; }
}