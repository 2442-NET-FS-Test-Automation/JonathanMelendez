using Library.Data;
using Library.Data.Entities;
using Microsoft.EntityFrameworkCore;

public interface ISeeder
{
    IReadOnlyList<int> SeedOrders(int n, bool expedited);    
}
public class Seeder(IDbContextFactory<LibraryDbContext> factory) : ISeeder 
{
    private readonly IDbContextFactory<LibraryDbContext> _factory = factory;
    private static readonly string[] Skus = {"BK-001", "BK-002", "BK-003"};

    // SO testing smth
    public IReadOnlyList<int> SeedOrders(int n, bool expedited) // SO testing smth2
    { // SO testing smth3
        // SO testing smth4
        using var db = _factory.CreateDbContext();

        var pid = db.Products.ToDictionary(p => p.Sku, p => p.Id);

        var ids = new List<int>(n);

        for (int i = 0; i < n; i++)
        {
            var order = new Order {
                CustomerId = Random.Shared.Next(1, 3),
                Priority = expedited ? Priority.Epedited : Priority.Normal,
                Lines = { new OrderLine { ProductId = pid[Skus[i % Skus.Length]], Quantity = 1 } }
            };

            db.Orders.Add(order);
            db.SaveChanges();
            ids.Add(order.Id);
        }
        return ids;
    }
}
