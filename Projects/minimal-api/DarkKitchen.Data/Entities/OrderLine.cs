using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DarkKitchen.Data.Entities;

[Table("OrderLines")]
public class OrderLine
{
    public int Id {get; set; }
    public int OrderId { get; set;}
    public int DishId {get; set;}
    public int Quantity { get; set; }
}