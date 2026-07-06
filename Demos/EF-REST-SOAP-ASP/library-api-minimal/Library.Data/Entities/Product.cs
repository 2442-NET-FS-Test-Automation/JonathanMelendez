using Microsoft.EntityFrameworkCore;
namespace Library.Data.Entities;

public class Product
{
    public int Id { get; set; }
    public string Sku { get; set; }
    public string Name { get; set; }
    // Data anotation to enforce constraints
    [Precision(10, 2)]
    public decimal Price { get; set; }

    // Collection to denote a relation
    public InventoryItem? Inventory { get; set; }
}