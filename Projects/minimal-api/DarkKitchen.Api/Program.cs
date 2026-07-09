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
app.MapGet("/dish-menu", (DarkKitchenDbContext db) => Results.Ok(db.Dishes.ToList()));

app.MapGet("/dish-menu/by-price", (DarkKitchenDbContext db) => 
    Results.Ok(db.Dishes
        .Select(d => new { d.Name, d.Price })
        .OrderByDescending(d => d.Price)
        .ToList()));

app.MapGet("/dish-menu/search/{id:int}", (int id, DarkKitchenDbContext db) =>
{
    var dish = db.Dishes.Where(d => d.Id == id).FirstOrDefault();
    if (dish == null) return Results.NotFound($"No dish found with id '{id}'");
    return Results.Ok(dish);
});

app.MapGet("/dish-menu/search/{name}", (string name, DarkKitchenDbContext db) =>
{
    var dishes = db.Dishes.Where(d => d.Name.ToLower().Contains(name.ToLower()));
    if (!dishes.Any()) return Results.NotFound($"No dishes found with name containing '{name}'");
    return Results.Ok(dishes);
});

app.MapPost("/dish-menu/toggle-enabled/{id:int}", async (int id, DarkKitchenDbContext db) =>
{
    var dish = await db.Dishes.FirstOrDefaultAsync(d => d.Id == id);
    if (dish == null) return Results.NotFound($"No dish found with id '{id}'");
    dish.Enabled = !dish.Enabled;
    await db.SaveChangesAsync();
    return Results.Ok($"Dish {dish.Name} is now {(dish.Enabled ? "enabled" : "disabled")}");
});


// Inventory stuff
app.MapGet("/inventory", (DarkKitchenDbContext db) => 
    Results.Ok(db.Ingredients
        .Select(i => new { i.Name, Stock = $"{i.Stock} {i.Unit.GetAbbreviation()}" })
        .ToList()));

app.MapGet("/inventory/search/{id:int}", (int id, DarkKitchenDbContext db) =>
{
    var ingredient =db.Ingredients
        .Where( i => i.Id == id)
        .Select(i => new { i.Name, Stock = $"{i.Stock} {i.Unit.ToString()}" })
        .FirstOrDefault();
    if (ingredient == null) return Results.NotFound($"No ingredient found with id '{id}'");
    return Results.Ok(ingredient);    
});

app.MapGet("/inventory/search/{name}", (string name, DarkKitchenDbContext db) =>
{
    var ingredients = db.Ingredients
        .Where( i => i.Name.ToLower().Contains(name.ToLower()))
        .Select(i => new { i.Name, Stock = $"{i.Stock} {i.Unit.ToString()}" });
    if (!ingredients.Any()) return Results.NotFound($"No dishes found with name containing '{name}'");
    return Results.Ok(ingredients);
});

app.MapGet("/inventory/by-stock", (DarkKitchenDbContext db) => 
    Results.Ok(db.Ingredients
        .Select(i => new { i.Name, i.Stock, Unit = i.Unit.ToString() })
        .OrderByDescending(i => i.Stock)
        .ToList()));

app.MapGet("/inventory/out-of-stock", (DarkKitchenDbContext db) => 
    Results.Ok(db.Ingredients
        .Select(i => new { i.Name, i.Stock })
        .Where(i => i.Stock <= 0.5m)
        .OrderBy(i => i.Stock)
        .ToList()));

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
app.MapGet("/orders", (DarkKitchenDbContext db) => 
    Results.Ok(db.Orders
        .Include(o => o.Lines)
        .ThenInclude(l => l.Dish)
        .Select(o => new { o.Id, o.CustomerId, Status = o.Status.ToString(), Lines = o.Lines.Select(l => new { l.DishId, l.Quantity }) })
        .ToList()));

app.MapGet("/orders/{orderId:int}", (int orderId, DarkKitchenDbContext db) =>
{
    var order = db.Orders
        .Include(o => o.Lines)
        .ThenInclude(l => l.Dish)
        .Select(o => new { o.Id, o.CustomerId, Status = o.Status.ToString(), Lines = o.Lines.Select(l => new { l.DishId, l.Quantity }) })
        .FirstOrDefault(o => o.Id == orderId);
    if (order == null) return Results.NotFound($"Order {orderId} not found");
    return Results.Ok(order);
});

app.MapGet("/orders/customer/{customerId:int}", (int customerId, DarkKitchenDbContext db) =>
{
    var orders = db.Orders
        .Where(o => o.CustomerId == customerId)
        .Include(o => o.Lines)
        .ThenInclude(l => l.Dish)
        .Select(o => new { o.Id, o.CustomerId, Status = o.Status.ToString(), Lines = o.Lines.Select(l => new { l.DishId, l.Quantity }) })
        .ToList();
    if (!orders.Any()) return Results.NotFound($"No orders found for customer {customerId}");
    return Results.Ok(orders);
});

app.MapPost("/orders/single", async (
    OrderPayload payload, 
    DarkKitchenDbContext db,
    IFulfillmentService fSvc,
    CancellationToken ct
) => {
    // We check that the customer exists
    if (await db.Customers.FirstOrDefaultAsync(c => c.Id == payload.CustomerId, ct) == null)
        return Results.BadRequest($"Customer {payload.CustomerId} not found");

    // We check that every Dish requested actually exists
    foreach (var line in payload.Lines)
    {
        var dish = await db.Dishes.FirstOrDefaultAsync(d => d.Id == line.DishId, ct);
        if (dish == null) return Results.BadRequest($"Dish {line.DishId} not found");
    }

    Order newOrder = new()
    {
        CustomerId = payload.CustomerId,
        Status = OrderStatus.Pending,
        Lines = payload.Lines.Select(l => new OrderLine { DishId = l.DishId, Quantity = l.Quantity }).ToList()
    };

    db.Orders.Add(newOrder);
    await db.SaveChangesAsync(ct);

    FulfillmentResult result = await fSvc.FulfillOneAsync(newOrder.Id, ct);

    return Results.Created($"/orders/{newOrder.Id}", new { OrderId = newOrder.Id, FulfillmentResult = result.ToString() });
});

app.MapPost("/orders/burst", () =>
{
    // Same as one but multiple and with generated orders
    return "You send a burst of orders here";
});


// Customers
app.MapGet("/customers", (DarkKitchenDbContext db) => {
    var customers = db.Customers
        .Select(c => new { c.Id, c.Name, c.Email })
        .ToList();
    return Results.Ok(customers);
});

app.MapGet("/customers/{customerId:int}", (int customerId, DarkKitchenDbContext db) => {
    var customer = db.Customers
        .Where(c => c.Id == customerId)
        .Select(c => new { c.Id, c.Name, c.Email })
        .FirstOrDefault();
    if (customer == null) return Results.NotFound($"Customer {customerId} not found");
    return Results.Ok(customer);
});

app.MapGet("/customers/search/{name}", (string name, DarkKitchenDbContext db) => {
    var customers = db.Customers
        .Where(c => c.Name.ToLower().Contains(name.ToLower()))
        .Select(c => new { c.Id, c.Name, c.Email })
        .ToList();
    if (!customers.Any()) return Results.NotFound($"No customers found with name containing '{name}'");
    return Results.Ok(customers);
});

app.MapPost("/customers", (CustomerCreatePayload payload, DarkKitchenDbContext db) =>
{
    var customer = new Customer
    {
        Name = payload.Name,
        Email = payload.Email
    };

    db.Customers.Add(customer);
    db.SaveChanges();

    return Results.Created($"/customers/{customer.Id}", customer);
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

app.MapGet("/reports/top-customers", (DarkKitchenDbContext db) =>
    Results.Ok(db.FulfillmentEvents
        .Where(f => f.Result == FulfillmentResult.Fulfilled)
        .Join(db.Orders, e => e.OrderId, o => o.Id, (e, o) => o)
        .GroupBy(o => o.CustomerId)
        .Select(g => new { CustomerId = g.Key, Orders = g.Count() })
        .Join(db.Customers, g => g.CustomerId, c => c.Id, (g, c) => new { c.Name, g.Orders })
        .OrderByDescending(x => x.Orders)
        .Take(3)
        .ToList()));

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
    return "Check for ingredients that have negative stock and report them";
});

app.Run();

Log.CloseAndFlush();

public record OrderLinePayload(int DishId, int Quantity);
public record OrderPayload(int CustomerId, List<OrderLinePayload> Lines);

public record CustomerCreatePayload(string Name, string Email);