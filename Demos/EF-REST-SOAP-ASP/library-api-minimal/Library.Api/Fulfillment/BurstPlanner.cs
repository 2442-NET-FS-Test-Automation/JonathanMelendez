using Library.Data.Entities;
using Library.Api;

namespace Library.Api.Fulfillment;

public class BurstPlanner
{
    public IReadOnlyList<int> OrderByPriority(IEnumerable<Order> orders)
    {
        PriorityQueue<int, int> pq = new();

        foreach (Order o in orders)
        {
            pq.Enqueue(o.Id, o.Priority == Priority.Expedited ? 0 : 1);
        }
        var orderByPriority = new List<int>();

        while (pq.TryDequeue(out int id, out _))
        {
            orderByPriority.Add(id);
        }
        return orderByPriority;
    }
}