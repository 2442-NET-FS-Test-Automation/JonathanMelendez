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
### Arrays
``` C#
// Definition syntax
string[] myArray = {"yes", "no"};
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