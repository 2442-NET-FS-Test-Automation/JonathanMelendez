# CSharp Basics
## Data Types and Operators

### Primitives
integer          byte, short, int, uint, long, ulong
floating point   float, double, decimal
boolean          bool (true, false)        
character        char

### Strings
In C# Strings is "immutable" thats why its a reference type variable

### Operators
Same as c++

## Control Flow
same as c++
if, else, else, if, switch

Switch expressions .NET 8 and forward
``` C#
string section =  genre switch
{
    // Expression body
    "Mystery => "Section A",
    "Science-Fiction" => "Section F"
};
Console.WriteLine(section);
```

### Loops
same as c++
``` C#
// Automatic item extraction
foreach (string book in books)
{
    //do something
}
```

## Structures
### Collections
#### Arrays
Fixed size
``` C#
// Definition syntax
string[] myArray = {"yes", "no"};
```
#### List<T>
Almost like a python array
- Indexed
- Dynamic size
- ordered
#### Stack<T>
A stack
- LIFO
#### Queue<T>
A queue
- FIFO
- Enqueue()
- Dequeue()
#### LinkedList<T>
Insert anywhere, reorder


#### HashSet
O(1) lookup time
Can't have duplicates
``` C#
private readonly HashSet<string> _authors = [];
```

### Classes
``` C#
public class Book
{
    // Atrributes automatic setter and getter
    public string Title {get; private set;}
    
    // By convention static attributes start with _
    public static int _nextId = 1;

    //constructor (An empty constructor is provided if not defined)
    public Book(string title)
    {
        Title = title;
    }

    // method static keyword not needed if is going to be for instances only
    public bool Checkout()
    {
        
    }
}
```

#### Partial classes
You can divide class definitions in multiple files by using `partial` keyword, just for mental organization, compiler will unify them.

#### Sealed classes
Using `sealed` keyword in a class definition will make it impossible to inherit from it

### Enums
Can only be one of the types specified
``` C#
public enum ItemKind
{
    Type1,
    Type2,
    Type3
}
```

### Struct
Small bundles of data with no identity
``` C#
public readonly struct ShelfLocation
{
    public int Aisle { get; }
    public int Shelf { get; }
    public ShelfLocation(int aisle, int shelf)
    {
        Aisle = aisle;
        Shelf = shelf;
    }
    public override string ToString()
    {
        return $"Aisle {Aisle}, Shelf {Shelf}";
    }
}
```


#### Method overriding
Theres 2 keywords that work for this
- override
- new

override, overwrites the function so it will replace it
new, hides the method so when calling from a parent object reference it will not be overriden

## Miscellaneus
No pointers yay, C# has a garbage collector

#### Formated string (python-like)
``` C#
$"Some text {variable}"
```

#### Shorthand functions
``` C#
private static decimal CalculateThing(int variable) => variable * 0.5;
```

#### Memory and value types
Value types get on stack. While Reference types get an address saved in the stack and the "values" get stored on the heap.

#### Nullable values
Question mark notes a variable can be nullable
``` C#
string? text;
```

#### Delegates
A delegate is a reference to a method in an argument list.
We use Predicate to pass a delegate to a function.
So in this Example match is a referene to a method.
``` C#
Find(Predicate<LibraryItem> match)

authorItems = Find(item => item.Author == "Frank Herbert");
```

### SOLID Principles
#### Single responsability
Keep things simple and focusing in just one function
#### Open / Closed
Open for modification-patching / closed for direct modification
#### Liskov Substitution
Parents/ childs should be substitutable
#### Inteface segregation
Single reponsability but interface
#### Dependency inversion
Not have direct dependencies