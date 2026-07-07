using Microsoft.EntityFrameworkCore;
using Serilog;

using DarkKitchen.Api.Fulfillment;
using DarkKitchen.Data.Entities;
using DarkKitchen.Data.Defaults;
using DarkKitchen.Data;

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


// Inventory stuff
app.MapGet("/dish-menu", (DarkKitchenDbContext db) => db.Dishes.ToList());

app.MapGet("/inventory", (DarkKitchenDbContext db) => db.Ingredients
        .Select(i => new { i.Name, Stock = $"{i.Stock} {i.Unit.GetAbbreviation()}" })
        .ToList());

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
app.MapGet("/orders/info", () =>
{
    return "Retrieve info about an specific order";
});

app.MapPost("/orders/single", () =>
{
    return "You place one order here";
});

app.MapPost("/orders/burst", () =>
{
    return "You send a burst of orders here";
});


// Reports
app.MapGet("/reports/top-products", () =>
{
    return "Ordered list of best selling products";
});

app.MapGet("/reports/worst-products", () =>
{
    return "Ordered list of worst selling products";
});

app.MapGet("/reports/top-customers", () =>
{
    return "Ordered list of most profitable customers";
});

app.MapGet("/reports/worst-customers", () =>
{
    return "Ordered list of worst profitable customers";
});

app.MapGet("/reports/fulfillment-rate", () =>
{
    return "Ordered list of worst selling products";
});


// Benchmarking
app.MapPost("/benchmark", () =>
{
    return "Test parallel vs sequential burst";
});

app.Run();

Log.CloseAndFlush();
