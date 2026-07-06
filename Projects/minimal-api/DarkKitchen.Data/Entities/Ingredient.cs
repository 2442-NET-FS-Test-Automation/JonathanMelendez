using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DarkKitchen.Data.Entities;

[Table("Ingredients")]
public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Stock { get; set; }
}