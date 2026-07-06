using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Serilog;
using Microsoft.EntityFrameworkCore.Metadata;


using Library.Data;
using Library.Data.Entities;
using Library.Api.Fulfillment;
using System.Diagnostics;

// Registering things with the builder
// Configuring things with the app
// Then the endpoints of the API calls

// Builder
var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Write to console, and write to a file - starting a new file each day.
    .WriteTo.File("logs/fulfillment-log-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog(); // Tell the builder to use Serilog for logging


// DbContext
var conn_string = "Server=localhost,1433;Database=LibraryMinimalDB;User Id=sa;Password=mssql65.;TrustServerCertificate=true";
builder.Services.AddDbContext<LibraryDbContext>(options => options.UseSqlServer(conn_string),
    ServiceLifetime.Scoped, ServiceLifetime.Singleton);
builder.Services.AddDbContextFactory<LibraryDbContext>(options => options.UseSqlServer(conn_string));

// Services
builder.Services.AddScoped<IFulfillmentService, FulfillmentService>();
builder.Services.AddScoped<ISeeder, Seeder>();
builder.Services.AddScoped<BurstPlanner>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
});

app.MapPost("/inventory/reset", (LibraryDbContext db, ILogger<Program> logger) =>
{
    logger.LogInformation("Started seeing database");

    foreach (InventoryItem inv in db.Inventory)
    {
        switch (inv.Id)
        {
            case 1:
                inv.CurrentStock = 5;
                break;
            case 2:
                inv.CurrentStock = 3;
                break;
            case 3:
                inv.CurrentStock = 8;
                break;
        }
    }
    db.SaveChanges();

    logger.LogInformation("Stock reset");
    return Results.Ok("Stock reset");
});


app.MapPost("/orders", async (
    OrderPayload orderRequest, 
    IDbContextFactory<LibraryDbContext> FactoryMethodBinding, 
    CancellationToken ct,
    IFulfillmentService fSvc
) => {
    await using var db = await FactoryMethodBinding.CreateDbContextAsync(ct);

    var newOrder = new Order
    {
        CustomerId = orderRequest.CustomerId,
        Priority = Priority.Normal,
        Lines = { new OrderLine { ProductId = orderRequest.ProductId, Quantity = orderRequest.Quantity } }
    };

    db.Orders.Add(newOrder);
    await db.SaveChangesAsync(ct);

    FulfillmentResult result = await fSvc.FulfillOneAsync(newOrder.Id, ct);

    return Results.Ok(new {orderId = newOrder.Id, result = result.ToString()});
});

app.MapPost("/orders/burst", (
    int n, 
    bool expedited, 
    ISeeder seeder, 
    IServiceScopeFactory scopes,
    IHostApplicationLifetime lifetime
) => {
    var ids = seeder.SeedOrders(n, expedited);

    var appStoping = lifetime.ApplicationStopping;

    _ = Task.Run( async () =>
    {
        try
        {
            using var scope = scopes.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IFulfillmentService>();
            await service.FulfillBurstAsync(ids, appStoping);
        }
        catch (Exception e)
        {
            Log.Error(e, "Burst fulfillment failed");
        }
    }, appStoping);
});

app.MapGet("/verify/no-oversell", (LibraryDbContext db) =>
{
    var rows = db.Inventory.Include(i => i.Product).ToList();
    var negative = rows.Where(i => i.CurrentStock < 0).ToList();
    var fulfilled = db.FulfillmentEvents.Count(e => e.Type == "Fulfilled");
    var backordered = db.FulfillmentEvents.Count(e => e.Type == "Backordered");

    return new { 
        anyNegative = negative.Any(),
        onHand = rows.Select(i => new {i.ProductId, i.CurrentStock}),
        unitsFulfilled = fulfilled,
        unitsBackordered = backordered 
    };
});


app.MapGet("/reports/by-completion", (LibraryDbContext db) =>
{
    return db.Orders
        .Where(o => o.Status == Status.Fulfilled)
        .OrderBy(o => o.CompletedUtc)
        .Select(o => new {o.Id, o.Priority, o.CompletedUtc})
        .ToList();
});

app.MapGet("/benchmark", async (int n, IFulfillmentService fs, ISeeder seeder, CancellationToken ct) =>
{
    var ids1 = seeder.ResetAndCreateOrders(n);
    
    // Sequential
    var sw1 = Stopwatch.StartNew();

    foreach (var id in ids1)
    {
        await fs.FulfillOneAsync(id, ct);
    }
    sw1.Stop();

    // Concurrent
    var ids2 = seeder.ResetAndCreateOrders(n);

    var sw2 = Stopwatch.StartNew();
    await fs.FulfillBurstAsync(ids2, ct);
    sw2.Stop();

    return new {
        sequentialMs = sw1.ElapsedMilliseconds,
        concurrentMs = sw2.ElapsedMilliseconds
    };

});

app.MapGet("/reports/top-products", (LibraryDbContext db) =>
{
    return db.FulfillmentEvents
        .Where(e => e.Type == "Fulfilled")
        .Join(db.OrderLines, e => e.OrderId, l => l.OrderId, (e, l) => l)
        .GroupBy(l => l.ProductId)
        .Select(g => new { ProductId = g.Key, Units = g.Sum(l => l.Quantity) })
        .OrderByDescending(x => x.Units)
        .ToList();
});

app.Run();
Log.CloseAndFlush();
public record OrderPayload(int ProductId, int Quantity, int CustomerId);