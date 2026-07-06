namespace Library.Data.Entities;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; } // FK -> Customer
    public Customer Customer { get; set; } = default!;
    public Priority Priority { get; set; }
    public Status Status { get; set; }
    public DateTime CreatedUtc {get; set; } = DateTime.UtcNow;
    public DateTime? CompletedUtc { get; set; }
    public List<OrderLine> Lines { get; set; } = [];
}