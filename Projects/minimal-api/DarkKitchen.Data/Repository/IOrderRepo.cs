using DarkKitchen.Data.Entities;

namespace DarkKitchen.Data.Repository;

public interface IOrderRepo
{
    // Orders
    Task<Order?> GetOrderByIdAsync(int orderId, CancellationToken ct);
    Task<List<Order>> GetOrdersByIdsAsync(IEnumerable<int> orderIds, CancellationToken ct);
    Task<List<Order>> GetOrdersForCustomerAsync(int customerId, CancellationToken ct);
    Task<List<Order>> GetAllOrdersAsync(CancellationToken ct);
    Task AddOrderAsync(Order order, CancellationToken ct);
    Task AddOrdersAsync(IEnumerable<Order> orders, CancellationToken ct);

    // Dishes
    Task<List<Dish>> GetAllDishesAsync(CancellationToken ct);
    Task<Dish?> GetDishByIdAsync(int dishId, CancellationToken ct);
    Task<List<Dish>> GetDishesByNameAsync(string searchedName, CancellationToken ct);
    Task<List<Dish>> GetEnabledDishesAsync(CancellationToken ct);
    Task<List<Dish>> GetDisabledDishesAsync(CancellationToken ct);
    Task<Dish> ToggleDishEnabledAsync(int dishId, CancellationToken ct);
}