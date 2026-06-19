using Store.Domain;
using Serilog;

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
                Console.WriteLine((selected == 3 ? "->" : "  ") + " Search by Category");
                Console.WriteLine((selected == 4 ? "->" : "  ") + " Close");
                break;
            case "ItemActions":
                Console.WriteLine("Item Actions Menu\n");
                Console.WriteLine("Select an option:");
                Console.WriteLine((selected == 0 ? "->" : "  ") + " Add Item");
                Console.WriteLine((selected == 1 ? "->" : "  ") + " Sell");
                Console.WriteLine((selected == 2 ? "->" : "  ") + " Restock");
                Console.WriteLine((selected == 3 ? "->" : "  ") + " Remove Item");
                Console.WriteLine((selected == 4 ? "->" : "  ") + " Close");
                break;
            case "CategoryAddMenu":
            case "CategoryMenu":
                if (menuName == "CategoryAddMenu") Console.WriteLine("Add Item\n");
                else Console.WriteLine("Item Search by Category\n");

                Console.WriteLine("Select a category:");
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
    public static int SelectMenu(string menu, int options, Func<int, bool> executeFunc)
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
        return selected;
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
                SelectMenu("ItemSearch", 4, ItemSearchExecute);
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
                Log.Information("App closed at {date}", DateTime.Now);
                Log.CloseAndFlush();
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
                ItemSearchId();
                EnterToContinue();
                break;
            case 1: // Search by Name
                ItemSearchName();
                EnterToContinue();
                break;
            case 2: // Search by Price
                ItemSearchPrice();
                EnterToContinue();
                break;
            case 3: // Search by Category
                ItemSearchCategory();
                EnterToContinue();
                break;
            case 4:
                return true;
        }
        return false;
    }
    public static bool ItemActionsExecute(int selected)
    {
        Console.Clear();
        switch (selected)
        {
            case 0: // Add Item
                ItemAdd();
                EnterToContinue();
                break;
            case 1: // Sell
                ItemSell();
                EnterToContinue();
                break;
            case 2: // Restock
                ItemRestock();
                EnterToContinue();
                break;
            case 3: // Remove Item
                ItemRemove();
                EnterToContinue();
                break;
            case 4:
                return true;
        }
        return false;
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