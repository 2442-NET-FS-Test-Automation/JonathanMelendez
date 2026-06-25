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

## Asyncronous code
async methods return `Task<T>`, that acts as a placeholder in memory for the method return when it finishes

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

## SQL

### SQL Sublanguages
#### DDL - Data Definition Language
- CREATE    > Creates new tables, schemas, databases
- Drop      > Deletes tables, schemas, databases, just like there werent there
- TRUNCATE  > Deletes all data in a table - preserves table structure
- ALTER     > Edit the structure of an existing table (columns, constraints, etc.)
#### DML - Data Manipulation Language
- INSERT    > Add new rows to a table
- UPDATE    > Update an existing row
- DELETE    > Remove a row
#### DQL - Data Query Language
- SELECT    > Retrieve a record a records from a database, table, etc.
              Can be used along a lot of keywords (GROUP BY, HAVING, WHERE, LIKE, etc.)
#### TCL - Transaction Control Language
- Begin
- Rollback
- Commit
#### DCL - Data Control Language
- Grant
- Revoke 

### Keys
Primary Key     - Chosen unique row ID
Candidate Key   - Any column(s) that could potentially be PK
Composite Key   - A key composed of 2+ keys (unique combination but not unique by themselves)
Foreign Key     - Column holding another table's PK
Unique Key      - No duplicates, but not used as PK
Alternate Key   - A candidate key not chosen as PK

### Table Normalization
#### 1NF
- Each cell MUST contain atomic values
- Each record represents one instance of an entity
- Remove repeating groups
- Identify keys

#### 2NF
- Be in 1NF
- Remove partial dependencies. (All non keys, depend entirely on the key (make more tables))

#### 3NF
- Be in 2NF
- Remove transitive dependencies (no non-key attributes should depend on another non key attribute)
