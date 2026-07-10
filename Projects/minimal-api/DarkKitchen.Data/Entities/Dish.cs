using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Data.Entities;

[Table("Dishes")]
public class Dish
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = default!;

    [Required, MaxLength(300)]
    public string Description { get; set; } = default!;

    [Precision(10, 2)]
    public decimal Price { get; set; } 

    public string OriginCountry { get; set; } = default!;
    
    public List<DishIngredient> Ingredients { get; set; } = [];
    public bool Enabled { get; set; } = true;
}