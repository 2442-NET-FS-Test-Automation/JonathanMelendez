using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Data.Entities;

[Table("Ingredients")]
public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    [Precision(10, 2)]
    public decimal Stock { get; set; }
    public byte[] RowVersion { get; set; } = default!;
}