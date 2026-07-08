using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DarkKitchen.Data.Entities;

[Table("Orders")]
public class Order
{
    public int Id {get; set;}
    public int CustomerId { get; set;} // FK -> Customer
    public Customer Customer { get; set; } = default!;
    public OrderPriority Priority { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime? CompletedUtc { get; set; }

    // Every Order has one or more OrderLines
    // Orderlines are the actual product and quantity of a something on the order. 
    public List<OrderLine> Lines { get; set; } = [];
}