using Store.Domain;

namespace Store.App;

public class Program
{
    private static List<Item> Items = GetSeedItems();
    private static List<string> History = [$"{DateTime.Now} - Program started"];
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
                Console.WriteLine("Jonnhy's Bank Main Menu");
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
                ItemAddMenu();
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
                ShowHistory();
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
        foreach (Item item in Items)
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
        foreach (Item item in Items)
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
    public static void ItemAddMenu()
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
                        ItemAdd(selected);
                        break;
                }
                Console.Clear();
                PrintMenu("CategoryMenu", selected);
            }
        }
    }
    public static void ItemAdd(int selected)
    {
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
        
        switch (selected)
        {
            case 0: // Clothing
                string[] sizes = ["CH", "M", "L", "XL", "XXL"];

                Console.Write($"Type the size (CH, M, L, XL, XXL): ");
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

                Items.Add(new Clothing(name, price, stock, size, color, material));
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
                
                Items.Add(new Electronic(name, price, stock, warranty, power));
                break;
            case 2: // Grocery
                Console.Write("Type the year of expiration: ");
                int year = 0;
                loop = true;
                while (loop)
                {
                    while(!int.TryParse(Console.ReadLine(), out year)) Console.Write("Try again: ");
                    if (!ValueCheck(year, DateTime.Now.Year, null)) Console.Write("Try again: ");
                    else loop = false;
                }
                
                Console.Write("Type the month of expiration: ");
                int month = 0;
                loop = true;
                while (loop)
                {
                    while(!int.TryParse(Console.ReadLine(), out month)) Console.Write("Try again: ");
                    if (!ValueCheck(month, 0, 12)) Console.Write("Try again: ");
                    else loop = false;
                }
                
                Console.Write("Type the day of expiration: ");
                int day = 0;
                loop = true;
                while (loop)
                {
                    while(!int.TryParse(Console.ReadLine(), out day)) Console.Write("Try again: ");
                    if (!ValueCheck(day, 0, 31)) Console.Write("Should be >= 0. Try again: ");
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
                
                Items.Add(new Grocery(name, price, stock, new DateOnly(year, month, day), weight));
                break;
        }
        AddHistory($"Added item with ID: {Items.Last().Id}");
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
            while(!int.TryParse(Console.ReadLine(), out id)) Console.Write($"Try again(1-{Items.Last().Id}): ");
            if (!ValueCheck(id, 1, Items.Last().Id)) Console.Write($"Try again (1-{Items.Last().Id}): ");
            else loop = false;
        }

        Console.Write("Type the amount sold: ");
        int amount = 0;
        loop = true;
        while (loop)
        {
            while(!int.TryParse(Console.ReadLine(), out amount)) Console.Write($"Try again(1-{Items.Last().Id}): ");
            if (!ValueCheck(amount, 0, null)) Console.Write($"Should be >= 0. Try again: ");
            else loop = false;
        }

        foreach(Item item in Items)
        {
            if (item.Id == id)
            {
                if (item.Sell(amount)) 
                {   
                    AddHistory($"Sold item with ID: {item.Id}");
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
            while(!int.TryParse(Console.ReadLine(), out id)) Console.Write($"Try again(1-{Items.Last().Id}): ");
            if (!ValueCheck(id, 1, Items.Last().Id)) Console.Write($"Try again (1-{Items.Last().Id}): ");
            else loop = false;
        }

        Console.Write("Type the amount to restock: ");
        int amount = 0;
        loop = true;
        while (loop)
        {
            while(!int.TryParse(Console.ReadLine(), out amount)) Console.Write($"Try again(1-{Items.Last().Id}): ");
            if (!ValueCheck(amount, 0, null)) Console.Write($"Should be >= 0. Try again: ");
            else loop = false;
        }

        foreach(Item item in Items) 
            if (item.Id == id)
            {
                AddHistory($"Restocked item with ID: {item.Id}");
                item.Restock(amount);
                Console.WriteLine($"Added {amount} to stock of {item.Name}");
                break;
            }
    }
    public static void ShowHistory()
    {
        Console.WriteLine("== Transactions History ==\n");
        foreach (string entry in History)
        {
            Console.WriteLine(entry);
        }
    }
    public static void AddHistory(string newEntry)
    {
        History.Add($"{DateTime.Now} - {newEntry}");
    }
    
    // Helper methods
    public static List<Item> GetSeedItems()
    {
        return new List<Item> {
            new Clothing("Shirt", 3, 10, "L", "White", "Poliester"),
            new Clothing("Pants", 4.5, 5, "XL", "Gray", "Silk"),
            new Electronic("Xbox Series Z", 500, 3, 2, 250),
            new Electronic("Potato Station", 800, 7, 1, 300),
            new Electronic("Televisor", 400, 12, 3, 50),
            new Grocery("Doritos", 0.8, 25, new DateOnly(2026, 10, 17), 0.2),
            new Grocery("Rice Bag", 0.8, 25, new DateOnly(2026, 10, 17), 1)
        };
    }
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