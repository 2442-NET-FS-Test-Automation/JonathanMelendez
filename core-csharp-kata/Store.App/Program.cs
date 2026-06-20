using Serilog;
using Store.Domain;

namespace Store.App;

public partial class Program
{
    private static IStoreRepository repository = new InMemStoreRepository();
    private static History THistory = new(repository);
    public static void Main()
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

        while (true) SelectMenu("MainMenu", OPTIONS_MAIN_MENU, MainMenuExecute);
    }
    public static void ItemList()
    {
        Console.WriteLine("All Item List\n");
        foreach (Item item in repository.GetAllItems())
        {
            Console.WriteLine(item);
            Console.WriteLine($"      {item.GetDetails()}");
        }
    }
    
    // Item Search
    public static void ItemSearchId()
    {
        int searchedId;
        Console.WriteLine("Item Search by Item ID\n");

        do Console.Write($"Type searched ID ({1}-{ItemFactory.getNextId}): ");
        while (!(int.TryParse(Console.ReadLine(), out searchedId) && ValueCheck(searchedId, 1, ItemFactory.getNextId)));

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
            Log.Error("Searched item with id {id} not found: {message}", e.Id, e.Message);
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
        while( rangeMin == null || rangeMax == null || (rangeMin > rangeMax));

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
    public static void ItemSearchCategory()
    {
        int selected = SelectMenu("CategorySearchMenu", OPTIONS_CATEGORY, option => true);
        
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
    public static void ItemAdd()
    {
        Console.WriteLine("Add Item");

        bool loop = true;

        Console.WriteLine();
        Console.Write("Type the name: ");
        string name = Console.ReadLine()!;

        Console.Write("Type the price: ");
        double price = 0;
        while (loop)
        {
            while(!double.TryParse(Console.ReadLine(), out price)) Console.Write("Try again: ");
            if (!ValueCheck(price, 0, null)) Console.Write("Should be >= 0. Try again: ");
            else loop = false;
        }
        
        Console.Write("Type the initial stock: ");
        int stock = 0; 
        loop = true;
        while (loop)
        {
            while(!int.TryParse(Console.ReadLine(), out stock)) Console.Write("Try again: ");
            if (!ValueCheck(stock, 0, null)) Console.Write("Should be >= 0. Try again: ");
            else loop = false;
        }
        
        int selected = SelectMenu("CategoryAddMenu", OPTIONS_CATEGORY, option => true);
        
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
                int warranty = 0;
                loop = true;
                while (loop)
                {
                    while(!int.TryParse(Console.ReadLine(), out warranty)) Console.Write("Try again: ");
                    if (!ValueCheck(warranty, 0, null)) Console.Write("Should be >= 0. Try again: ");
                    else loop = false;
                }
                
                Console.Write("Type the power consumption: ");
                int power = 0;
                loop = true;
                while (loop)
                {
                    while(!int.TryParse(Console.ReadLine(), out power)) Console.Write("Try again: ");
                    if (!ValueCheck(power, 0, null)) Console.Write("Should be >= 0. Try again: ");
                    else loop = false;
                }
                
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
                double weight = 0;
                loop = true;
                while (loop)
                {
                    while(!double.TryParse(Console.ReadLine(), out weight)) Console.Write("Try again: ");
                    if (!ValueCheck(weight, 0, null)) Console.Write("Should be >= 0. Try again: ");
                    else loop = false;
                }
                
                repository.AddItem(ItemFactory.Create(ItemKind.Grocery, name, price, stock, expirationDate: date, weightKg: weight));
                break;
            case 3: // Pokemon
                // TODO: Consumption API
                int gameId = 1;
                string pokeTypes = "normal";

                repository.AddItem(ItemFactory.Create(ItemKind.Pokemon, name, price, stock, pokeId: gameId, pokeType: pokeTypes));
                break;
            case 4:
                Console.WriteLine("\nItem creation cancelled!");
                return;
        }
        THistory.Add(TransactionEnum.Add, repository.GetLastItem());
        Console.WriteLine("\nItem added succesfully!");
    }
    public static void ItemRemove()
    {
        //TODO
        Console.Write("Type the ID of the item to remove: ");
        int id = 0;
        bool loop = true;
        while (loop)
        {
            while(!int.TryParse(Console.ReadLine(), out id)) Console.Write($"Try again(1-{ItemFactory.getNextId-1}): ");
            if (!ValueCheck(id, 1, ItemFactory.getNextId-1)) Console.Write($"Try again (1-{ItemFactory.getNextId-1}): ");
            else loop = false;
        }
        Item item = repository.GetItemById(id);
        if (repository.RemoveById(id))
        {
            THistory.Add(TransactionEnum.Remove, item);
            Console.WriteLine("Item Removed Succesfully");
        }
    }
    public static void ItemSell()
    {
        Console.Write("Type the ID of the item sold: ");
        int id = 0;
        bool loop = true;
        while (loop)
        {
            while(!int.TryParse(Console.ReadLine(), out id)) Console.Write($"Try again(1-{ItemFactory.getNextId-1}): ");
            if (!ValueCheck(id, 1, ItemFactory.getNextId-1)) Console.Write($"Try again (1-{ItemFactory.getNextId-1}): ");
            else loop = false;
        }

        Console.Write("Type the amount sold: ");
        int amount = 0;
        loop = true;
        while (loop)
        {
            while(!int.TryParse(Console.ReadLine(), out amount)) Console.Write($"Try again(1-{ItemFactory.getNextId-1}): ");
            if (!ValueCheck(amount, 0, null)) Console.Write($"Should be >= 0. Try again: ");
            else loop = false;
        }

        foreach(Item item in repository.GetAllItems())
        {
            if (item.Id == id)
            {
                if (item.Sell(amount)) 
                {   
                    THistory.Add(TransactionEnum.Sell, item, amount);
                    Console.WriteLine($"Sold {amount} of {item.Name}");
                }
                else Console.WriteLine("There is not enough stock!");
                break;
            }
        }
    }
    public static void ItemRestock()
    {
        Console.Write("Type the ID of the item to restock: ");
        int id = 0;
        bool loop = true;
        while (loop)
        {
            while(!int.TryParse(Console.ReadLine(), out id)) Console.Write($"Try again(1-{ItemFactory.getNextId}): ");
            if (!ValueCheck(id, 1, ItemFactory.getNextId-1)) Console.Write($"Try again (1-{ItemFactory.getNextId}): ");
            else loop = false;
        }

        Console.Write("Type the amount to restock: ");
        int amount = 0;
        loop = true;
        while (loop)
        {
            while(!int.TryParse(Console.ReadLine(), out amount)) Console.Write($"Try again(1-{ItemFactory.getNextId}): ");
            if (!ValueCheck(amount, 0, null)) Console.Write($"Should be >= 0. Try again: ");
            else loop = false;
        }

        foreach(Item item in repository.GetAllItems()) 
            if (item.Id == id)
            {
                THistory.Add(TransactionEnum.Restock, item, amount);
                item.Restock(amount);
                Console.WriteLine($"Added {amount} to stock of {item.Name}");
                break;
            }
    }
    
    // Helper methods
    public static bool ValueCheck(int value, int? lowRange, int? highRange)
    {
        if (lowRange != null) if (value < lowRange) return false;
        if (highRange != null) if (value > highRange) return false;
        return true;
    }
    public static bool ValueCheck(double value, double? lowRange, double? highRange)
    {
        if (lowRange != null) if (value < lowRange) return false;
        if (highRange != null) if (value > highRange) return false;
        return true;
    }
}