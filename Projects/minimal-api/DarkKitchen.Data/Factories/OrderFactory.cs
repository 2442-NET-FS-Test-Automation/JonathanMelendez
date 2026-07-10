using DarkKitchen.Data.Entities;

namespace DarkKitchen.Data.Factories;

public class OrderFactory
{
    public Order CreateOrder(string priority, int customerId, IEnumerable<(int dishId, int qty)> lines)
    {
        return priority switch
        {
            "normal" => BuildOrder(OrderPriority.Normal, customerId, lines),
            "urgent" => BuildOrder(OrderPriority.Urgent, customerId, lines),
            _ => throw new ArgumentException($"Unknown order priority: {priority}"),
        };
    }

    private Order BuildOrder(OrderPriority priority, int customerId, IEnumerable<(int dishId, int qty)> lines)
    {
        return new Order
        {
            CustomerId = customerId,
            Priority = priority,
            Status = OrderStatus.Pending,
            Lines = lines.Select(l => new OrderLine
            {
                DishId = l.dishId,
                Quantity = l.qty
            }).ToList()
        };
    }
}