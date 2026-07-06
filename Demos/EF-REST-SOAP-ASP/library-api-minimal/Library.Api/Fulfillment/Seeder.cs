using Library.Data;
using Library.Data.Entities;
using Microsoft.EntityFrameworkCore;

public interface ISeeder
{
    IReadOnlyList<int> SeedOrders(int n, bool expedited); 
    IReadOnlyList<int> ResetAndCreateOrders(int n);   
}
public class Seeder(IDbContextFactory<LibraryDbContext> factory) : ISeeder 
{
    private readonly IDbContextFactory<LibraryDbContext> _factory = factory;
    private static readonly string[] Skus = {"BK-001", "BK-002", "BK-003"};

    public IReadOnlyList<int> SeedOrders(int n, bool expedited)
    { 
        using var db = _factory.CreateDbContext();

        var pid = db.Products.ToDictionary(p => p.Sku, p => p.Id);

        var ids = new List<int>(n);

        for (int i = 0; i < n; i++)
        {
            var order = new Order {
                CustomerId = Random.Shared.Next(1, 3),
                Priority = expedited ? Priority.Expedited : Priority.Normal,
                Lines = { new OrderLine { ProductId = pid[Skus[i % Skus.Length]], Quantity = 1 } }
            };

            db.Orders.Add(order);
            db.SaveChanges();
            ids.Add(order.Id);
        }
        return ids;
    }

    public IReadOnlyList<int> ResetAndCreateOrders(int n)
    {
        using var db = _factory.CreateDbContext();
        foreach (InventoryItem inv in db.Inventory)
        {
            switch (inv.ProductId)
            {
                case 1:
                    inv.CurrentStock = 5;
                    break;
                case 2:
                    inv.CurrentStock = 3;
                    break;
                case 3:
                    inv.CurrentStock = 8;
                    break;
            }
        }
        db.SaveChanges();

        var pid = db.Products.ToDictionary(p => p.Sku, p => p.Id);

        var ids = new List<int>(n);

        for(int i = 0; i < n; i++)
        {
            var order = new Order
            {
                CustomerId = Random.Shared.Next(1, 3),
                Priority = i % 3 == 0 ? Priority.Expedited : Priority.Normal,
                Lines = { new OrderLine { 
                        ProductId = pid[new [] {"BK-001", "BK-002", "BK-003"}[i % 3]], 
                        Quantity = 1 } }
            };
            db.Orders.Add(order);
            db.SaveChanges();
            ids.Add(order.Id);
        }
        return ids;
    }
}
