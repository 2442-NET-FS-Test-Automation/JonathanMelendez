using DarkKitchen.Data.Entities;

namespace DarkKitchen.Data.Repository;

public interface IOrderRepo
{
    Task<Order?> GetOrderByIdAsync(int orderId, CancellationToken ct);
    Task<List<Order>> GetOrdersByIdsAsync(IEnumerable<int> orderIds, CancellationToken ct);
    Task<List<Order>> GetOrdersForCustomerAsync(int customerId, CancellationToken ct);
    Task<List<Order>> GetAllOrdersAsync(CancellationToken ct);
    Task AddOrderAsync(Order order, CancellationToken ct);
    Task AddOrdersAsync(IEnumerable<Order> orders, CancellationToken ct);
}