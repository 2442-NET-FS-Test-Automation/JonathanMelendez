using Store.Domain;

namespace Store.App;

public class Program
{
    private static IStoreRepository repository = new InMemStoreRepository();
    private static History THistory = new();
    public static void Main()
    {
        bool isRunning = true;
        int selected = 0;

        Console.Clear();
        PrintMenu("MainMenu", selected);
        
        while (isRunning)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    // Handle arrow movement in menu
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.RightArrow:
                        selected++;
                        if (selected > 6) selected = 0;
                        break;

                    case ConsoleKey.UpArrow:
                    case ConsoleKey.LeftArrow:
                        selected--;
                        if (selected < 0) selected = 6;
                        break;

                    case ConsoleKey.Enter:
                        ExecuteMainMenuSelected(selected);
                        break;
                }
                Console.Clear();
                PrintMenu("MainMenu", selected);
            }
        }
    }
    public static void PrintMenu(string menuName, int selected)
    {   
        switch (menuName)
        {
            case "MainMenu":
                Console.WriteLine("Store Main Menu");
                Console.WriteLine("Select an option:");
                Console.WriteLine((selected == 0 ? "->" : "  ") + " List Items");
                Console.WriteLine((selected == 1 ? "->" : "  ") + " Search Item");
                Console.WriteLine((selected == 2 ? "->" : "  ") + " Add Item");
                Console.WriteLine((selected == 3 ? "->" : "  ") + " Sell");
                Console.WriteLine((selected == 4 ? "->" : "  ") + " Restock");
                Console.WriteLine((selected == 5 ? "->" : "  ") + " Transaction History");
                Console.WriteLine((selected == 6 ? "->" : "  ") + " Close");
                break;
            case "CategoryMenu":
                Console.WriteLine("== Add Item ==\n");
                Console.WriteLine("Select a category for the new item.");
                Console.WriteLine((selected == 0 ? "->" : "  ") + " Clothing");
                Console.WriteLine((selected == 1 ? "->" : "  ") + " Electronic");
                Console.WriteLine((selected == 2 ? "->" : "  ") + " Grocery");
                Console.WriteLine((selected == 3 ? "->" : "  ") + " Cancel");
                break;
            default:
                Console.WriteLine($"{menuName} not implemented");
                break;

        }
        
    }
    public static void ExecuteMainMenuSelected(int selected)
    {
        Console.Clear();
        switch (selected)
        {
            case 0:
                ItemList();
                EnterToContinue();
                break;
            case 1:
                ItemSearch();
                EnterToContinue();
                break;
            case 2:
                ItemAdd();
                break;
            case 3:
                ItemSell();
                EnterToContinue();
                break;
            case 4:
                ItemRestock();
                EnterToContinue();
                break;
            case 5:
                // TODO: History implementations
                EnterToContinue();
                break;
            case 6:
                System.Environment.Exit(0);
                break;
        }
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
    public static void ItemSearch()
    {
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
    public static int ItemCategorySelector()
    {
        bool isRunning = true;
        int selected = 0;

        Console.Clear();
        PrintMenu("CategoryMenu", selected);
        
        while (isRunning)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    // Handle arrow movement in menu
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.RightArrow:
                        selected++;
                        if (selected > 3) selected = 0;
                        break;

                    case ConsoleKey.UpArrow:
                    case ConsoleKey.LeftArrow:
                        selected--;
                        if (selected < 0) selected = 3;
                        break;

                    case ConsoleKey.Enter:
                        if (selected == 3) 
                        {
                            isRunning = false;
                            break;
                        }
                        return selected;
                }
                Console.Clear();
                PrintMenu("CategoryMenu", selected);
            }
        }
        return 0;
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
        EnterToContinue();
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
                    THistory.Add(TransactionEnum.Sell, item);
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
                THistory.Add(TransactionEnum.Restock, item);
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
    public static void EnterToContinue()
    {
        bool enter = false;

        Console.WriteLine("\nPress Enter to continue...");

        while (!enter)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) enter = true;
            }
        }
    }
}