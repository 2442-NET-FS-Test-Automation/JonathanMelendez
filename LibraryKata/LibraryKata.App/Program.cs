using System.Linq.Expressions;
using LibraryKata.Domain;

namespace LibraryKata.App;

public class Program
{
    public static void Main()
    {
        ClassesExample();
        OopDemo();
        CollectionsDemo();
    }

    private static void ClassesExample()
    {
        Console.WriteLine("Using Book Class");
        Book dune = new Book("Dune", "Frank Herbert", 5);
        Book littleprince = new("The Little Prince", "The Author of The Little Prince", 0);

        Console.WriteLine(dune);
        Console.WriteLine(littleprince.ToString());

        Console.WriteLine($"Checking out Dune: {dune.Checkout()}");
        Console.WriteLine($"Checking out The Little Prince: {littleprince.Checkout()}");

    }

    public static void OopDemo()
    {
        Console.WriteLine("\n\n == OOP DEMO ==");

        LibraryItem[] catalog =
        {
            new Book("Dune", "The Dune Author", 5),
            new ReferenceBook("Dictionary", "RAE", "Language"),
            new Magazine("Sports Ilustrated", "Conde Naste", 3)
        };

        foreach (LibraryItem item in catalog)
        {
            Console.WriteLine(item);
        }

        foreach (LibraryItem item in catalog)
        {
            if(item is ILendable lendable)
            {
                Console.WriteLine($"This item can be borrowed, checkout -> {lendable.Checkout()}");
            }
            else
            {
                Console.WriteLine("Reference only");
            }
        }

        Magazine wired = new Magazine("Wired", "Conde Naste", 5);
        LibraryItem baseMag = wired;

        Console.WriteLine("\n\n == override vs new overriding ==");
        Console.WriteLine($"Magazine -> {wired.ShelfLabel()}");
        Console.WriteLine($"LibraryItem -> {baseMag.ShelfLabel()}");

    }

        private static void CollectionsDemo()
    {
        Console.WriteLine("== Collections demo stuuuff");

        Catalog catalog = new();
        catalog._items.Add(new Book("Dune", "Frank Herbert", 5));
        catalog._items.Add(new ReferenceBook("Dictionary", "RAE", "Language"));
        catalog._items.Add(new Magazine("Wired", "Conde Naste", 5));

        Console.WriteLine($"Catalog holds {catalog.Count}; first is {catalog._items[0].Title}");

        ItemKind kind = ItemKind.Magazine;

        ShelfLocation location = new(3,12);
        Console.WriteLine($"{kind} sits at {location}");


        Shelf<LibraryItem> shelf = new(2);
        Shelf<int> intShelf = new(15);

        shelf.TryAdd(new Book("Dune", "Frank Herbert", 5));
        shelf.TryAdd(catalog._items[2]);

        Console.WriteLine($"Adding more when size is {shelf.Capacity}: {shelf.TryAdd(catalog._items[1])}");
    }
}