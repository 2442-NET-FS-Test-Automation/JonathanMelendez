namespace Store.App;
public partial class Program
{
    public static void PrintMenu(string menuName, int selected)
    {   
        switch (menuName)
        {
            case "MainMenu":
                Console.WriteLine("Store Main Menu\n");
                Console.WriteLine("Select an option:");
                Console.WriteLine((selected == 0 ? "->" : "  ") + " List Items");
                Console.WriteLine((selected == 1 ? "->" : "  ") + " Search Items");
                Console.WriteLine((selected == 2 ? "->" : "  ") + " Item Actions");
                Console.WriteLine((selected == 3 ? "->" : "  ") + " Transaction Undo");
                Console.WriteLine((selected == 4 ? "->" : "  ") + " Close");
                break;
            case "ItemSearch":
                Console.WriteLine("Item Search Menu\n");
                Console.WriteLine("Select an option:");
                Console.WriteLine((selected == 0 ? "->" : "  ") + " Search by ID");
                Console.WriteLine((selected == 1 ? "->" : "  ") + " Search by Name");
                Console.WriteLine((selected == 2 ? "->" : "  ") + " Search by Price");
                Console.WriteLine((selected == 3 ? "->" : "  ") + " Close");
                break;
            case "ItemActions":
                Console.WriteLine("Item Actions Menu\n");
                Console.WriteLine("Select an option:");
                Console.WriteLine((selected == 0 ? "->" : "  ") + " Sell");
                Console.WriteLine((selected == 1 ? "->" : "  ") + " Restock");
                Console.WriteLine((selected == 2 ? "->" : "  ") + " Add Item");
                Console.WriteLine((selected == 3 ? "->" : "  ") + " Remove Item");
                Console.WriteLine((selected == 4 ? "->" : "  ") + " Close");
                break;
            case "CategoryMenu":
                Console.WriteLine("== Add Item ==\n");
                Console.WriteLine("Select a category for the new item:");
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
    public static void SelectMenu(string menu, int options, Func<int, bool> executeFunc)
    {
        bool isRunning = true;
        int selected = 0;

        Console.Clear();
        PrintMenu(menu, selected);
        
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
                        if (selected > options) selected = 0;
                        break;

                    case ConsoleKey.UpArrow:
                    case ConsoleKey.LeftArrow:
                        selected--;
                        if (selected < 0) selected = options;
                        break;

                    case ConsoleKey.Enter:
                        if(executeFunc(selected)) isRunning = false;
                        break;
                }
                Console.Clear();
                PrintMenu(menu, selected);
            }
        }
    }
    public static bool MainMenuExecute(int selected)
    {
        Console.Clear();
        switch (selected)
        {
            case 0:
                ItemList();
                EnterToContinue();
                break;
            case 1:
                SelectMenu("ItemSearch", 3, ItemSearchExecute);
                break;
            case 2:
                SelectMenu("ItemActions", 4, ItemActionsExecute);
                break;
            case 3:
                // TODO: Make a menu and make it fancier
                THistory.Undo();
                EnterToContinue();
                break;
            case 4:
                System.Environment.Exit(0);
                break;
        }
        return true;
    }
    public static bool ItemSearchExecute(int selected)
    {
        Console.Clear();
        switch (selected)
        {
            case 0: // Search by ID
                ItemSearch(selected);
                EnterToContinue();
                break;
            case 1: // Search by Name
                ItemSearch(selected);
                EnterToContinue();
                break;
            case 2: // Search by Price
                ItemSearch(selected);
                EnterToContinue();
                break;
            case 3:
                return true;
        }
        return false;
    }
    public static bool ItemActionsExecute(int selected)
    {
        Console.Clear();
        switch (selected)
        {
            case 0:// Sell
                ItemSell();
                EnterToContinue();
                break;
            case 1: // Restock
                ItemRestock();
                EnterToContinue();
                break;
            case 2: // Add Item
                ItemAdd();
                EnterToContinue();
                break;
            case 3: // Remove Item
                // TODO: Item remove
                break;
            case 4:
                return true;
        }
        return false;
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