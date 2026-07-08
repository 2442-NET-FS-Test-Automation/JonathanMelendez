using Microsoft.EntityFrameworkCore;
using Serilog;

using DarkKitchen.Api.Fulfillment;
using DarkKitchen.Data.Entities;
using DarkKitchen.Data.Defaults;
using DarkKitchen.Data;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// DB Stuff
var conn_string = "Server=localhost,1433;Database=DarkKitchenDB;User Id=sa;Password=mssql65.;TrustServerCertificate=true";
builder.Services.AddDbContext<DarkKitchenDbContext>(options => options.UseSqlServer(conn_string),
    ServiceLifetime.Scoped, ServiceLifetime.Singleton);

// Services
builder.Services.AddDbContextFactory<DarkKitchenDbContext>(options => options.UseSqlServer(conn_string));
builder.Services.AddScoped<IFulfillmentService, FulfillmentService>();

// Logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/darkkitchen-api-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// dih menu
app.MapGet("/dish-menu", (DarkKitchenDbContext db) => db.Dishes.ToList());

app.MapGet("/dish-menu/search/{name}", (string name, DarkKitchenDbContext db) => 
    db.Dishes.Where(d => d.Name.ToLower().Contains(name.ToLower())));


// Inventory stuff
app.MapGet("/inventory", (DarkKitchenDbContext db) => db.Ingredients
        .Select(i => new { i.Name, Stock = $"{i.Stock} {i.Unit.GetAbbreviation()}" })
        .ToList());

app.MapGet("/inventory/search/{id:int}", (int id, DarkKitchenDbContext db) => db.Ingredients
        .Where( i => i.Id == id)
        .Select(i => new { i.Name, Stock = $"{i.Stock} {i.Unit.ToString()}" }));

app.MapGet("/inventory/search/{name}", (string name, DarkKitchenDbContext db) => db.Ingredients
        .Where( i => i.Name.ToLower().Contains(name.ToLower()))
        .Select(i => new { i.Name, Stock = $"{i.Stock} {i.Unit.ToString()}" }));

app.MapGet("/inventory/by-stock", (DarkKitchenDbContext db) => db.Ingredients
        .Select(i => new { i.Name, i.Stock, i.Unit })
        .OrderByDescending(i => i.Stock)
        .ToList());

app.MapGet("/inventory/out-of-stock", (DarkKitchenDbContext db) => db.Ingredients
        .Select(i => new { i.Name, i.Stock })
        .Where(i => i.Stock <= 0.5m)
        .OrderBy(i => i.Stock)
        .ToList());

app.MapPost("/inventory/reset", (DarkKitchenDbContext db, ILogger<Program> logger) =>
{
    foreach(var ingredient in db.Ingredients)
    {
        if (IngredientDefaults.InitialStocks.TryGetValue(ingredient.Id, out decimal initialStock))
        {
            ingredient.Stock = initialStock;
            Log.Information("Reset ingredient {id} to {stock} stock", ingredient.Id, initialStock);
        }
    }
    Log.Information("Applying reset changes...");
    db.SaveChanges();
    return Results.Ok("Stock reset");
});


// Orders
app.MapGet("/orders/{orderId:int}", (int orderId, DarkKitchenDbContext db) =>
    db.Orders
        .Include(o => o.Lines)
        .ThenInclude(l => l.Dish)
        .Where(o => o.Id == orderId)
        .ToList());

app.MapPost("/orders/single", () =>
{
    return "You place one order here";
});

app.MapPost("/orders/burst", () =>
{
    return "You send a burst of orders here";
});


// Reports
app.MapGet("/reports/top-products", (DarkKitchenDbContext db) =>
    db.FulfillmentEvents
        .Where(f => f.Result == FulfillmentResult.Fulfilled)
        .Join(db.OrderLines, e => e.OrderId, l => l.OrderId, (e, l) => l)
        .GroupBy(l => l.DishId)
        .Select(g => new { DishId = g.Key, Units = g.Sum(l => l.Quantity) })
        .Join(db.Dishes, g => g.DishId, d => d.Id, (g, d) => new {g.Units, d.Name})
        .OrderByDescending(x => x.Units)
        .Take(3)
        .ToList());

app.MapGet("/reports/worst-products", (DarkKitchenDbContext db) =>
    db.FulfillmentEvents
        .Where(f => f.Result == FulfillmentResult.Fulfilled)
        .Join(db.OrderLines, e => e.OrderId, l => l.OrderId, (e, l) => l)
        .GroupBy(l => l.DishId)
        .Select(g => new { DishId = g.Key, Units = g.Sum(l => l.Quantity) })
        .Join(db.Dishes, g => g.DishId, d => d.Id, (g, d) => new {g.Units, d.Name})
        .OrderBy(x => x.Units)
        .Take(3)
        .ToList());

app.MapGet("/reports/top-customers", (DarkKitchenDbContext db) =>
{
    return "Ordered list of most profitable customers";
});

app.MapGet("/reports/worst-customers", (DarkKitchenDbContext db) =>
{
    return "Ordered list of worst profitable customers";
});

app.MapGet("/reports/fulfillment-rate", (DarkKitchenDbContext db) =>
    db.FulfillmentEvents
    .GroupBy(g => g.Result)
    .Select(g => new { Result = g.Key.ToString(), Quantity = g.Count()})
    .ToList());


// Benchmarking and tests
app.MapPost("/benchmark", () =>
{
    return "Test parallel vs sequential burst";
});

app.MapGet("/verify/no-oversell", () =>
{
    return "Test parallel vs sequential burst";
});

app.Run();

Log.CloseAndFlush();
