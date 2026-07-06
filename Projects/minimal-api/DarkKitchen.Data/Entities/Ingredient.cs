using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DarkKitchen.Data.Entities;

[Table("Ingredients")]
public class Ingredient
{
    public int Id { get; set; }
    public int Name { get; set; } 
    public int Stock { get; set; } 
}