using Microsoft.EntityFrameworkCore;
using Library.Data;
using Microsoft.OpenApi;
using Library.Data.Entities;
// Registering things with the builder
// Configuring things with the app
// Then the endpoints of the API calls

// Builder
var builder = WebApplication.CreateBuilder(args);

var conn_string = "Server=localhost,1433;Database=LibraryMinimalDB;User Id=sa;Password=mssql65.;TrustServerCertificate=true";
builder.Services.AddDbContext<LibraryDbContext>(options => options.UseSqlServer(conn_string));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// App
var app = builder.Build();

// Swagger stuff
app.UseSwagger();
app.UseSwaggerUI();


// Endpoints
app.MapGet("/", () => "Hello World!");

// Get all items from inventory
app.MapGet("/inventory", async (LibraryDbContext db) => (
    await db.Inventory.ToListAsync()
));

// Using LINQ - Language Integrated Query
app.MapGet("/inventory/by-value", (LibraryDbContext db) => (
    db.Inventory.Include(i => i.Product)
        .GroupBy(i => i.CurrentStock >= 5 ? "well-stocked" : "low")
        .Select(g => new { tier = g.Key, count = g.Count(), units = g.Sum(i => i.CurrentStock) })
        .ToList()
));


// peek endpoints are usually for demo/debugging, are real productionized API would have no reason to expose http endpoints
app.MapGet("/peek/tracking", (LibraryDbContext db) =>
{
    var unchanged = db.Products.First();
    var modified = db.Products.Skip(1).First();

    modified.Price += 1;

    db.Products.Add(new Library.Data.Entities.Product {Sku = "BK-TMP", Name = "tmp", Price = 1m});

    var states = db.ChangeTracker.Entries()
    .Select(e => new { entity = e.Entity.GetType().Name, state = e.State.ToString()})
    .ToList();

    db.ChangeTracker.Clear();

    return states;
});

app.MapGet("/peek/conflict", (IServiceScopeFactory scopes) =>
{
    // Manually asking for scopes, so we can assign them manually
    // Normally ASP.NET tracks and assigns them during runtime
    using var scopeA = scopes.CreateScope();
    using var scopeB = scopes.CreateScope();

    var firstDb = scopeA.ServiceProvider.GetRequiredService<LibraryDbContext>();
    var secondDb = scopeB.ServiceProvider.GetRequiredService<LibraryDbContext>();

    var firstInventory = firstDb.Inventory.First(i => i.Id == 1);
    var secondInventory = secondDb.Inventory.First(i => i.Id == 1);

    firstInventory.CurrentStock++;
    firstDb.SaveChanges();

    secondInventory.CurrentStock++;
    try
    {
        secondDb.SaveChanges();
        return Results.Ok("No conflicts");
    }
    catch (DbUpdateConcurrencyException e)
    {
        // Retry the update
        var entry = e.Entries.Single();

        // Grab the current values from the db
        var current = entry.GetDatabaseValues();
        // every entry stores the values when was instantiated, and the new ones
        // with this one get the database actual values and insert them
        entry.OriginalValues.SetValues(current!);

        ((InventoryItem)entry.Entity).CurrentStock = current!.GetValue<int>(nameof(InventoryItem.CurrentStock)) + 1;
        secondDb.SaveChanges();
        return Results.Ok("Conflict caught, reloaded and retried");
    }
    
    return Results.Ok();
});


app.Run();
