using System.Diagnostics;
using System.Runtime.InteropServices;
using Serilog;

namespace Store.Domain;

public class InMemStoreRepository : IStoreRepository
{
    private static Dictionary<int, Item> _items = [];
    public InMemStoreRepository()
    {
        GenSeedItemsAsync().GetAwaiter().GetResult();
        
    } 
    public void AddItem(Item item)
    {
        try
        {
            _items.Add(item.Id, item); // This one throws when duplicates
        }
        catch (Exception e)
        {
            Log.Error("ItemAdd Error: {message}", e.Message);
            return;
        }

        Log.Information("Added {Name} with id {id}", item.Name, item.Id);

    }
    public List<Item> GetAllItems() => _items.Values.ToList();
    public Item GetLastItem() => _items.Last().Value;
    public Item GetItemById(int id)
    {
        if (_items.TryGetValue(id, out Item? item)) return item;

        Log.Warning("Lookup failed for {id}", id);
        throw new ItemNotFoundException(id);
    }
    public IEnumerable<Item> Find(Predicate<Item> match)
    {
        foreach (Item item in _items.Values)
        {
            if(match(item)) yield return item;
        }
    }
    public bool RemoveById(int id)
    {
        if (_items.Remove(id))
        {
            Log.Information("Removed item with id {id}", id);
            return true;
        }
        
        Log.Information("Item with id {id} removed/not found", id);
        return false;
    }

    private async Task GenSeedItemsAsync()
    {
        AddItem(ItemFactory.Create(ItemKind.Clothing, "Shirt", 3, 10, "L", "White", "Poliester"));
        AddItem(ItemFactory.Create(ItemKind.Clothing, "Pants", 4.5, 5, "XL", "Gray", "Silk"));
        AddItem(ItemFactory.Create(ItemKind.Electronics, "Xbox Series Z", 500, 3, warrantyYears: 2, powerConsumption: 250));
        AddItem(ItemFactory.Create(ItemKind.Electronics, "Potato Station", 800, 7, warrantyYears: 1, powerConsumption: 300));
        AddItem(ItemFactory.Create(ItemKind.Electronics, "Televisor", 400, 12, warrantyYears: 3, powerConsumption: 50));
        AddItem(ItemFactory.Create(ItemKind.Grocery, "Doritos", 0.8, 25, expirationDate: new DateOnly(2026, 10, 17), weightKg: 0.2));
        AddItem(ItemFactory.Create(ItemKind.Grocery, "Rice Bag", 0.8, 25, expirationDate: new DateOnly(2026, 10, 17), weightKg: 1));
        
        PokeApiClient client = new();

        Log.Information("Starting Task.WhenAll to verify OVERLAP...");
        
        Task<(int, string)?> itemPikachu = client.FetchByNameAsync("pikachu");
        Log.Information("   -> [PIKACHU] Request HTTP sent");

        Task<(int, string)?> itemCharizard = client.FetchByNameAsync("charizard");
        Log.Information("   -> [CHARIZARD] Request HTTP sent");

        // IMPORTANT: Waiting for both
        await Task.WhenAll(itemPikachu, itemCharizard);

        var resultPikachu = itemPikachu.Result;
        var resultCharizard = itemCharizard.Result;

        if (resultPikachu is not null && resultCharizard is not null)
        {
            var (PokeIds, pokeTypes) = resultPikachu.Value;
            AddItem(ItemFactory.Create(ItemKind.Pokemon, "pikachu", 100.50, 2, 
                pokeId: PokeIds, 
                pokeType: pokeTypes));

            (PokeIds, pokeTypes) = resultCharizard.Value;
            AddItem(ItemFactory.Create(ItemKind.Pokemon, "charizard", 250.00, 1, 
                pokeId: PokeIds, 
                pokeType: pokeTypes));
        }
    }
}