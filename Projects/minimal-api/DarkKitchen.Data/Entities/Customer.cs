using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DarkKitchen.Data.Entities;

[Table("Customers")]
public class Customer
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = default!;
    
    [Required, MaxLength(256)]
    public string Email { get; set; } = default!;

    //One customer can have many orders
    public List<Order> Orders { get; set; } = [];
}