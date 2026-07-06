using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Data.Entities;

[Table("Dishes")]
public class Dish
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    [Precision(10, 2)]
    public decimal Price { get; set; } 
    public List<Ingredient> Ingredients { get; set; } = [];
    public bool Enabled { get; set; } = true;
}