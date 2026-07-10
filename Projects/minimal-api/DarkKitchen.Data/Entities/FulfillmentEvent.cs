using System.ComponentModel.DataAnnotations.Schema;


namespace DarkKitchen.Data.Entities;

[Table("FulfillmentEvents")]
public class FulfillmentEvent
{
    public int Id { get; set;}
    public int OrderId { get; set;}
    public FulfillmentResult Result { get; set; } = default!;
    public DateTime FulfilledAtUtc { get; set; }
}