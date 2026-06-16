using System.Linq.Expressions;
using LibraryKata.Domain;

namespace LibraryKata.App;

public class Program
{
    public static void Main()
    {
        ClassesExample();
        OopDemo();
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
        Console.WriteLine("==== COLLECTIONS DEMO STUFF =====");

        //Creating a catalog object
        // Because this is backed by a list, it grows and shrinks for us
        Catalog catalog = new();

        // I could create my objects
        Book dune = new Book("Dune", "Frank Herbert", 3);

        // Then add them - we now go through Catalog.Add(), which wraps the private list.
        // We never touch catalog._items directly anymore: the list is the Catalog's business.
        catalog.Add(dune);

        // I can also just call a constructor inside the Add() method call
        // Methods having their arguments satisfied by the return of other methods is a common pattern
        // and sometimes you'll get like 4-5 callbacks deep in tools like ASP.NET
        catalog.Add(new ReferenceBook("C# Language Specs", "Microsoft", "Technology"));
        catalog.Add(new Magazine("Nat Geo", "Nat Geo", 4));

        // Count is a wrapper property; catalog[0] uses the indexer - reads like an array,
        // but it's read-only, so no one can do catalog[0] = somethingElse.
        Console.WriteLine($"Catalog holds {catalog.Count}; first is {catalog[0].Title}");

        // The other containers, each reached through intent-named methods instead of raw fields:
        // STACK (LIFO) - return cart: the last book dropped is the first re-shelved.
        catalog.DropInReturnCart(catalog[0]);
        catalog.DropInReturnCart(catalog[2]);
        Console.WriteLine($"Return cart has {catalog.CartCount}; reshelving \"{catalog.Reshelve().Title}\" first");

        // QUEUE (FIFO) - holds line: the first member to ask is the first served.
        catalog.PlaceHold("Ada");
        catalog.PlaceHold("Grace");
        Console.WriteLine($"{catalog.HoldsWaiting} holds waiting; serving {catalog.ServeNextHold()} first");

        // LINKEDLIST - a reading list we reorder; AddNextUp jumps to the front.
        catalog.AddToReadingList(catalog[0]);
        catalog.AddNextUp(catalog[1]);
        Console.WriteLine("Reading list order:");
        foreach (LibraryItem item in catalog.ReadingList)
        {
            Console.WriteLine($"  - {item.Title}");
        }

        // Enum + Struct use
        ItemKind kind = ItemKind.Magazine; // example of selecting an enum value
        
        ShelfLocation location = new ShelfLocation(3,12); // struct - looks alot like a class, but it is a VALUE type

        Console.WriteLine($"{kind} sits at {location}"); 

        Book duneCopy = dune; // copies the reference
        // lets say I modify duneCopy, what happens to the data in dune?
        // all we copied was the pointer - these two things are not independent

        ShelfLocation location2 = location; // copies the data/fields 
        // these are not linked in the same way, I can edit the data in one without touching the other

        // Generics: our own Shelf<T> that can hold anything - though technically all the collections
        // we used thusfar have been generic classes themselves
        Shelf<LibraryItem> shelf = new Shelf<LibraryItem>(2);
        Shelf<int> intShelf = new Shelf<int>(200);
        
        shelf.TryAdd(catalog[0]);
        shelf.TryAdd(catalog[1]);

        Console.WriteLine($"Trying to add a third thing in our catalog: {shelf.TryAdd(catalog[2])}");
        
        
    }

}
