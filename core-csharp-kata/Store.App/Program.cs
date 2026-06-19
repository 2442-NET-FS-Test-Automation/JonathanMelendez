using Store.Domain;

namespace Store.App;

public partial class Program
{
    private static IStoreRepository repository = new InMemStoreRepository();
    private static History THistory = new(repository);
    public static void Main()
    {
        while (true) SelectMenu("MainMenu", 4, MainMenuExecute);
    }
    public static void ItemList()
    {
        Console.WriteLine("== All Item List ==\n");
        foreach (Item item in repository.GetAllItems())
        {
            Console.WriteLine(item);
            Console.WriteLine($"      {item.GetDetails()}");
        }
    }
    public static void ItemSearch(int selected)
    {
        // TODO: 
        // search by id (selected = 0)
        // search by name (selected = 1)
        // search by price (selected = 2)
        string searchedName;
        do
        {
            Console.WriteLine("== Item Search ==\n");

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
        Console.WriteLine($"== Items containing '{searchedName}' ==\n");
        int numMatches = 0;
        foreach (Item item in repository.GetAllItems())
        {
            if (item.Name.ToLower().Contains(searchedName.ToLower()))
            {
                Console.WriteLine(item);
                Console.WriteLine($"      {item.GetDetails()}");
                numMatches++;
            }
        }
        Console.WriteLine($"\n{numMatches} matches found.");
    }
    public static void ItemAdd()
    {
        Console.WriteLine("== Add Item ==");

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

        int selected = ItemCategorySelector();
        
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
        }
        THistory.Add(TransactionEnum.Add, repository.GetLastItem());
        Console.WriteLine("\nItem added succesfully!");
    }
    public static void ItemSell()
    {
        Console.Write("Type the ID of the item sold: ");
        int id = 0;
        bool loop = true;
        while (loop)
        {
            while(!int.TryParse(Console.ReadLine(), out id)) Console.Write($"Try again(1-{Item._nextId-1}): ");
            if (!ValueCheck(id, 1, Item._nextId-1)) Console.Write($"Try again (1-{Item._nextId-1}): ");
            else loop = false;
        }

        Console.Write("Type the amount sold: ");
        int amount = 0;
        loop = true;
        while (loop)
        {
            while(!int.TryParse(Console.ReadLine(), out amount)) Console.Write($"Try again(1-{Item._nextId-1}): ");
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
            while(!int.TryParse(Console.ReadLine(), out id)) Console.Write($"Try again(1-{Item._nextId-1}): ");
            if (!ValueCheck(id, 1, Item._nextId-1)) Console.Write($"Try again (1-{Item._nextId-1}): ");
            else loop = false;
        }

        Console.Write("Type the amount to restock: ");
        int amount = 0;
        loop = true;
        while (loop)
        {
            while(!int.TryParse(Console.ReadLine(), out amount)) Console.Write($"Try again(1-{Item._nextId-1}): ");
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