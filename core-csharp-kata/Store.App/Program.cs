using Serilog;
using Store.Domain;
using System.Numerics;

namespace Store.App;

public partial class Program
{
    private static IStoreRepository repository = new InMemStoreRepository();
    private static History THistory = new(repository);
    public static async Task Main()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File("logs/Store.txt", 
                rollingInterval: RollingInterval.Day, // Creates a new file each day
                retainedFileCountLimit: 31,           // Cleans up logs older than 31 days
                fileSizeLimitBytes: 10485760,         // 10 MB limit per file
                rollOnFileSizeLimit: true)            // Rolls to a new file if size limit is met
            .CreateLogger();

        Log.Information("App started at {date}", DateTime.Now);

        while (true) await SelectMenu("MainMenu", OPTIONS_MAIN_MENU, MainMenuExecute);
    }
    public static void ItemList(IStoreRepository.OrderType match)
    {
        Console.WriteLine($"Item List {match}\n");
        foreach (Item item in repository.OrderList(match))
        {
            Console.WriteLine(item);
            Console.WriteLine($"      {item.GetDetails()}");
        }
    }
    
    // Item Search
    public static void ItemSearchId()
    {
        Console.WriteLine("Item Search by Item ID\n");
        Console.Write($"Type searched ID ({1}-{ItemFactory.getNextId-1}): ");
        int searchedId = ValueCheck<int>(1, ItemFactory.getNextId-1, $"Try again(1-{ItemFactory.getNextId-1}): ");

        try
        {
            Item searchedItem = repository.GetItemById(searchedId);

            Console.Clear();
            Console.WriteLine("Item Search by Item ID\n");
            Console.WriteLine($"Item data found:");
            
            foreach (KeyValuePair<string, string> entry in searchedItem.Describe())
            {
                Console.WriteLine($"{entry.Key, 10}: {entry.Value}");
            }

        }
        catch (ItemNotFoundException e)
        {
            Console.WriteLine("Item not found. Sorry.");
            Log.Error("Searched item with id {id} not found: {message}", e.Id, e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine("Something went wrong. Sorry.");
            Log.Error("Exception found: {Message}", e.Message);
        }
    }
    public static void ItemSearchName()
    {
        string searchedName;
        do
        {
            Console.WriteLine("Item Search by Item Name\n");

            Console.Write("Type search term: ");
            searchedName = Console.ReadLine()!;
            if (searchedName.Length < 1)
            {
                Console.WriteLine("Search term can't be empty!");
                Thread.Sleep(500);
                Console.Clear();
            }
        }
        while (searchedName.Length < 1);

        Console.Clear();
        Console.WriteLine($"Items containing '{searchedName}'\n");
        int numMatches = 0;

        foreach (Item item in repository.Find(item => item.Name.ToLower().Contains(searchedName.ToLower())))
        {
            Console.WriteLine(item);
            Console.WriteLine($"      {item.GetDetails()}");
            numMatches++;
        }
        Console.WriteLine($"\n{numMatches} matches found.");
    }
    public static void ItemSearchPrice()
    {
        double? rangeMin = null;
        double? rangeMax = null;
        do
        { 
            Console.Clear();
            Console.WriteLine("Item Search by Item Price Range\n");
            Console.Write("Type search min range: ");
            rangeMin = double.TryParse(Console.ReadLine(), out double min_)? min_ : null;
            Console.Write("Type search max range: ");
            rangeMax = double.TryParse(Console.ReadLine(), out double max_)? max_ : null;
            Console.Clear();
        }
        while(rangeMin == null || rangeMax == null || (rangeMin > rangeMax));

        Console.WriteLine($"Item Search by Item Price Range ({rangeMin}-{rangeMax})\n");

        int numMatches = 0;
        foreach (Item item in repository.Find(item => item.Price >= rangeMin && item.Price <= rangeMax))
        {
            Console.WriteLine(item);
            Console.WriteLine($"      {item.GetDetails()}");
            numMatches++;
        }
        Console.WriteLine($"\n{numMatches} matches found.");
    }
    public static async Task ItemSearchCategory()
    {
        int selected = await SelectMenu("CategorySearchMenu", OPTIONS_CATEGORY, option => Task.FromResult(true));
        
        string inputCategory = selected switch
        {
            0   => "Clothing",
            1   => "Electronic",
            2   => "Groceries",
            3   => "Pokemon",
            _  => "Return"
        };
        if (inputCategory == "Return") return;
        
        Console.Clear();
        Console.WriteLine($"Item Search by Category - {inputCategory}\n");

        int numMatches = 0;
        foreach (Item item in repository.Find(item => item.Category == inputCategory))
        {
            Console.WriteLine(item);
            Console.WriteLine($"      {item.GetDetails()}");
            numMatches++;
        }
        Console.WriteLine($"\n{numMatches} matches found.");
    }

    // Item Actions
    public static async Task ItemAdd()
    {   
        bool loop = true;
        Console.WriteLine("Add Item\n");
        int selected = await SelectMenu("CategoryAddMenu", OPTIONS_CATEGORY, option => Task.FromResult(true));
        if (selected == 4)
        {
            Console.WriteLine("\nItem creation cancelled!");
            return;
        }

        Console.Write(selected == 3 ? "\nType the pokemon name to fetch: " : "\nType the item name: ");
        string name = Console.ReadLine()!;

        Console.Write("Type the price: ");
        double price = ValueCheck<double>(0, null);
        
        Console.Write("Type the initial stock: ");
        int stock = ValueCheck<int>(0, null);
        
        bool isSuccesfully = true;
        
        switch (selected)
        {
            case 0: // Clothing
                string[] sizes = ["CH", "M", "L", "XL", "XXL"];

                Console.Write($"\nType the size (CH, M, L, XL, XXL): ");
                loop = true;
                string size = "";
                while (loop)
                {
                    size = Console.ReadLine()!;
                    foreach (string s in sizes)
                        if (s == size) 
                        {
                            loop = false;
                            break;
                        }
                    if (loop) Console.Write("Try again: ");
                }

                Console.Write("Type the color: ");
                string color = Console.ReadLine()!;

                Console.Write("Type the material: ");
                string material = Console.ReadLine()!;

                repository.AddItem(ItemFactory.Create(ItemKind.Clothing, name, price, stock, size, color, material));
                break;
            case 1: // Electronic
                Console.Write("Type years of warranty: ");
                int warranty = ValueCheck<int>(0, null);
                
                Console.Write("Type the power consumption: ");
                int power = ValueCheck<int>(0, null);
                
                repository.AddItem(ItemFactory.Create(ItemKind.Electronics, name, price, stock, warrantyYears: warranty, powerConsumption: power));
                break;
            case 2: // Grocery
                Console.Write("Type the expiration date (yyyy-MM-dd): ");
                DateOnly date = new();
                loop = true;
                while (loop)
                {
                    if(!DateOnly.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", out date)) Console.Write("Try again: ");
                    else loop = false;
                }
                
                Console.Write("Type the weight: ");
                double weight = ValueCheck<double>(0, null);
                
                repository.AddItem(ItemFactory.Create(ItemKind.Grocery, name, price, stock, expirationDate: date, weightKg: weight));
                break;
            case 3: // Pokemon
                PokeApiClient client = new();
                var pokeResult = await client.FetchByNameAsync(name);
                
                if (pokeResult != null)
                {
                    (int gameId, string pokeTypes) = pokeResult.Value;
                    // Console.WriteLine($"Adding new pokemon {name}, {price}, {stock}, {gameId}, {pokeTypes}");
                    repository.AddItem(ItemFactory.Create(ItemKind.Pokemon, name, price, stock, pokeId: gameId, pokeType: pokeTypes));
                }
                else isSuccesfully = false;
                break;
        }
        if (isSuccesfully) {
            THistory.Add(TransactionEnum.Add, repository.GetLastItem());
            Console.WriteLine(selected == 3 ? "\nPokemon added succesfully!" : "\nItem added succesfully!");
        }
        else Console.WriteLine("\nPokemon NOT added, fetch failed!");
    }
    public static void ItemRemove()
    {
        Console.WriteLine("Item Remove\n");
        Console.Write("Type the ID of the item to remove: ");
        int id = ValueCheck<int>(1, ItemFactory.getNextId-1, $"Try again(1-{ItemFactory.getNextId-1}): ");
        try
        {
            Item item = repository.GetItemById(id);
            if (repository.RemoveById(id))
            {
                THistory.Add(TransactionEnum.Remove, item);
                Console.WriteLine("\nItem Removed Succesfully");
                return;
            }
        }
        catch (ItemNotFoundException e)
        {
            Console.WriteLine("\nItem not found. Sorry.");
            Log.Error("Tried to remove non existing item with id {id}: {message}", e.Id, e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine("\nSomething went wrong. Sorry.");
            Log.Error("Exception found: {Message}", e.Message);
        }
    }
    public static void ItemSell()
    {
        Console.WriteLine("Item Sell\n");
        Console.Write("Type the ID of the item sold: ");
        int id = ValueCheck<int>(1, ItemFactory.getNextId-1, $"Try again(1-{ItemFactory.getNextId-1}): ");

        Console.Write("Type the amount sold: ");
        int amount = ValueCheck<int>(0, null);

        Item item;
        try // Shouldnt fail but just in case
        {
            item = repository.GetItemById(id);
            if (item.Sell(amount))
            {   
                THistory.Add(TransactionEnum.Sell, item, amount);
                Console.WriteLine($"Sold {amount} of {item.Name}");
            }
            else Console.WriteLine("There is not enough stock!");
        } 
        catch (ItemNotFoundException e)
        {
            Console.WriteLine("\nItem not found. Sorry.");
            Log.Warning("Item with {id} not found: {Message}", e.Id, e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine("\nSomething went wrong. Sorry.");
            Log.Error("Exception found: {Message}", e.Message);
        }
        
    }
    public static void ItemRestock()
    {
        Console.WriteLine("Item Restock\n");
        Console.Write("Type the ID of the item to restock: ");
        int id = ValueCheck<int>(1, ItemFactory.getNextId-1, $"Try again(1-{ItemFactory.getNextId-1}): ");

        Console.Write("Type the amount to restock: ");
        int amount = ValueCheck<int>(0, null);
        
        try
        {
            Item item = repository.GetItemById(id);
            item.Restock(amount);
            THistory.Add(TransactionEnum.Restock, item, amount);
            Console.WriteLine($"Added {amount} to stock of {item.Name}");
        }
        catch (ItemNotFoundException e)
        {
            Console.WriteLine("\nItem not found. Sorry.");
            Log.Error("Tried to restock non existing item with id {id}: {message}", e.Id, e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine("\nSomething went wrong. Sorry.");
            Log.Error("Exception found: {Message}", e.Message);
        }
    }
    
    // Helper methods
    public static T ValueCheck<T>(T? lowRange, T? highRange, string? message = null) where T : struct, INumber<T>
    {
        bool loop = true;
        T value = T.Zero;
        while (loop)
        {
            while(!T.TryParse(Console.ReadLine(), null, out value)) 
                Console.Write("Try again: ");
            if (!ValueInRange<T>(value, lowRange, highRange))
            {
                if (message != null) Console.Write(message);
                else Console.Write($"{((lowRange != null) ? $"Should be >= {lowRange}. " : "")}Try again: ");
            }
            
            else loop = false;
        }
        return value;
    }
    public static bool ValueInRange<T>(T value, T? lowRange, T? highRange) where T : struct, INumber<T>
    {
        if (lowRange != null) if (value < lowRange) return false;
        if (highRange != null) if (value > highRange) return false;
        return true;
    }
}