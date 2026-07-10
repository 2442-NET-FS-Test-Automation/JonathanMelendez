using DarkKitchen.Data.Entities;

namespace DarkKitchen.Data.Repository;

public interface IDarkKitchenRepo
{
    // Dishes
    Task<List<Dish>> GetAllDishesAsync(CancellationToken ct);
    Task<Dish?> GetDishByIdAsync(int dishId, CancellationToken ct);
    Task<List<Dish>> GetDishesByIdsAsync(IEnumerable<int> dishesIds, CancellationToken ct);
    Task<List<Dish>> GetDishesByNameAsync(string searchedName, CancellationToken ct);
    Task<List<Dish>> GetEnabledDishesAsync(CancellationToken ct);
    Task<List<Dish>> GetDisabledDishesAsync(CancellationToken ct);
    Task<Dish> ToggleDishEnabledAsync(int dishId, CancellationToken ct);

    // Inventory
    Task<List<Ingredient>> GetAllIngredientsAsync(CancellationToken ct);
    Task<Ingredient?> GetIngredientByIdAsync(int id, CancellationToken ct);
    Task<List<Ingredient>> GetIngredientsByNameAsync(string searchedName, CancellationToken ct);
    Task<List<Ingredient>> GetIngredientsBelowStockAsync(decimal minStock, CancellationToken ct);
    Task IngredientsResetStock(CancellationToken ct);

    // Orders
    Task<Order?> GetOrderByIdAsync(int orderId, CancellationToken ct);
    Task<List<Order>> GetOrdersByIdsAsync(IEnumerable<int> orderIds, CancellationToken ct);
    Task<List<Order>> GetOrdersForCustomerAsync(int customerId, CancellationToken ct);
    Task<List<Order>> GetAllOrdersAsync(CancellationToken ct);
    Task AddOrderAsync(Order order, CancellationToken ct);
    Task AddOrdersAsync(IEnumerable<Order> orders, CancellationToken ct);

    // Customers
    Task<List<Customer>> GetAllCustomersAsync(CancellationToken ct);
    Task<Customer?> GetCustomerByIdAsync(int customerId, CancellationToken ct);
    Task<List<Customer>> GetCustomersByIdsAsync(IEnumerable<int> customersIds, CancellationToken ct);
    Task<List<Customer>> GetCustomerByNameAsync(string searchedName, CancellationToken ct);
    Task<Customer> AddCustomerAsync(Customer customer, CancellationToken ct);
    Task DeleteCustomerByIdAsync(int customerId, CancellationToken ct);
}