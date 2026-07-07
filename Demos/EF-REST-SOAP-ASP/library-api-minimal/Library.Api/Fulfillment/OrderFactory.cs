using Library.Data.Entities;

namespace Library.Api.Fulfillment;

public class OrderFactory(IFulfillmentService fs)
{
    private readonly IFulfillmentService _fs = fs;

    public Order CreateOrder(string kind, int customerId, IEnumerable<(string sku, int qty)> lines)
    {
        switch (kind)
        {
            case "normal":
                return BuildOrder(Priority.Normal, customerId, lines);
            case "expedited":
                return BuildOrder(Priority.Expedited, customerId, lines);
            default:
                throw new ArgumentException($"Wrong kind: {kind}");
        }
        
    }
    public Order BuildOrder(Priority priority, int customerId, IEnumerable<(string sku, int qty)> lines)
    {
        return new Order
        {
            CustomerId = customerId,
            Priority = priority,
            Status = Status.Pending,
            Lines = lines.Select(l => new OrderLine { 
                ProductId = _fs.ResolveProductId(l.sku),
                Quantity = l.qty
            }).ToList()
        };
    }
}