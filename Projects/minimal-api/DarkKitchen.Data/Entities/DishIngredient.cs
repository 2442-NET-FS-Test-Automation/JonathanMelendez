using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace DarkKitchen.Data.Entities;

public class DishIngredient
{
    public int Id { get; set; }

    [Required]
    public int DishId { get; set; }

    [Required]
    public int IngredientId { get; set; }

    [Precision(10,3)]
    public decimal Quantity { get; set; }
}