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
app.MapGet("/dish-menu", async (DarkKitchenDbContext db) => Results.Ok(db.Dishes.ToListAsync()));

app.MapGet("/dish-menu/by-price", async (DarkKitchenDbContext db) => 
    Results.Ok(await db.Dishes
        .Select(d => new { d.Name, d.Price })
        .OrderByDescending(d => d.Price)
        .ToListAsync()));

app.MapGet("/dish-menu/search/{id:int}", async (int id, DarkKitchenDbContext db) =>
{
    var dish = await db.Dishes.Where(d => d.Id == id).FirstOrDefaultAsync();
    if (dish == null) return Results.NotFound($"No dish found with id '{id}'");
    return Results.Ok(dish);
});

app.MapGet("/dish-menu/search/{name}", async (string name, DarkKitchenDbContext db) =>
{
    var dishes = await db.Dishes.Where(d => d.Name.ToLower().Contains(name.ToLower())).ToListAsync();
    if (dishes.Count() == 0) return Results.NotFound($"No dishes found with name containing '{name}'");
    return Results.Ok(dishes);
});

app.MapPost("/dish-menu/toggle-enabled/{id:int}", async (int id, DarkKitchenDbContext db) =>
{
    var dish = await db.Dishes.FirstOrDefaultAsync(d => d.Id == id);
    if (dish == null) return Results.NotFound($"No dish found with id '{id}'");
    dish.Enabled = !dish.Enabled;
    await db.SaveChangesAsync();
    Log.Information("Dish {DishId} is now {Enabled}", dish.Id, dish.Enabled);
    return Results.Ok($"Dish {dish.Name} is now {(dish.Enabled ? "enabled" : "disabled")}");
});


// Inventory stuff
app.MapGet("/inventory", async (DarkKitchenDbContext db) => 
    Results.Ok(await db.Ingredients
        .Select(i => new { i.Name, Stock = $"{i.Stock} {i.Unit.GetAbbreviation()}" })
        .ToListAsync()));

app.MapGet("/inventory/search/{id:int}", async (int id, DarkKitchenDbContext db) =>
{
    var ingredient =await db.Ingredients
        .Where( i => i.Id == id)
        .Select(i => new { i.Name, Stock = $"{i.Stock} {i.Unit.ToString()}" })
        .FirstOrDefaultAsync();
    if (ingredient == null) return Results.NotFound($"No ingredient found with id '{id}'");
    return Results.Ok(ingredient);    
});

app.MapGet("/inventory/search/{name}", async (string name, DarkKitchenDbContext db) =>
{
    var ingredients = await db.Ingredients
        .Where( i => i.Name.ToLower().Contains(name.ToLower()))
        .Select(i => new { i.Name, Stock = $"{i.Stock} {i.Unit.ToString()}" })
        .ToListAsync();
    if (ingredients.Count == 0) return Results.NotFound($"No ingredients found with name containing '{name}'");
    return Results.Ok(ingredients);
});

app.MapGet("/inventory/by-stock", async (DarkKitchenDbContext db) => 
    Results.Ok(await db.Ingredients
        .Select(i => new { i.Name, i.Stock, Unit = i.Unit.ToString() })
        .OrderByDescending(i => i.Stock)
        .ToListAsync()));

app.MapGet("/inventory/out-of-stock", async (DarkKitchenDbContext db) => 
    Results.Ok(await db.Ingredients
        .Select(i => new { i.Name, i.Stock })
        .Where(i => i.Stock <= 0.5m)
        .OrderBy(i => i.Stock)
        .ToListAsync()));

app.MapPost("/inventory/reset", async (DarkKitchenDbContext db, ILogger<Program> logger) =>
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
    await db.SaveChangesAsync();
    return Results.Ok("Stock reset");
});


// Orders
app.MapGet("/orders", async (DarkKitchenDbContext db) => 
    Results.Ok(await db.Orders
        .Include(o => o.Lines)
        .ThenInclude(l => l.Dish)
        .Select(o => new { o.Id, o.CustomerId, Status = o.Status.ToString(), Lines = o.Lines.Select(l => new { l.DishId, l.Quantity }) })
        .ToListAsync()));

app.MapGet("/orders/{orderId:int}", async (int orderId, DarkKitchenDbContext db) =>
{
    var order = await db.Orders
        .Include(o => o.Lines)
        .ThenInclude(l => l.Dish)
        .Select(o => new { o.Id, o.CustomerId, Status = o.Status.ToString(), Lines = o.Lines.Select(l => new { l.DishId, l.Quantity }) })
        .FirstOrDefaultAsync(o => o.Id == orderId);
    if (order == null) return Results.NotFound($"Order {orderId} not found");
    return Results.Ok(order);
});

app.MapGet("/orders/customer/{customerId:int}", async (int customerId, DarkKitchenDbContext db) =>
{
    var orders = await db.Orders
        .Where(o => o.CustomerId == customerId)
        .Include(o => o.Lines)
        .ThenInclude(l => l.Dish)
        .Select(o => new { o.Id, o.CustomerId, Status = o.Status.ToString(), Lines = o.Lines.Select(l => new { l.DishId, l.Quantity }) })
        .ToListAsync();
    if (orders.Count == 0) return Results.NotFound($"No orders found for customer {customerId}");
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
    Log.Information("Created order {OrderId} for customer {CustomerId}", newOrder.Id, newOrder.CustomerId);
    await db.SaveChangesAsync(ct);

    FulfillmentResult result = await fSvc.FulfillOneAsync(newOrder.Id, ct);

    return Results.Created($"/orders/{newOrder.Id}", new { OrderId = newOrder.Id, FulfillmentResult = result.ToString() });
});

app.MapPost("/orders/burst", async () =>
{
    // Same as one but multiple and with generated orders
    return "You send a burst of orders here";
});


// Customers
app.MapGet("/customers", async (DarkKitchenDbContext db) => {
    var customers = await db.Customers
        .Select(c => new { c.Id, c.Name, c.Email })
        .ToListAsync();
    return Results.Ok(customers);
});

app.MapGet("/customers/{customerId:int}", async (int customerId, DarkKitchenDbContext db) => {
    var customer = await db.Customers
        .Where(c => c.Id == customerId)
        .Select(c => new { c.Id, c.Name, c.Email })
        .FirstOrDefaultAsync();
    if (customer == null) return Results.NotFound($"Customer {customerId} not found");
    return Results.Ok(customer);
});

app.MapGet("/customers/search/{name}", async (string name, DarkKitchenDbContext db) => {
    var customers = await db.Customers
        .Where(c => c.Name.ToLower().Contains(name.ToLower()))
        .Select(c => new { c.Id, c.Name, c.Email })
        .ToListAsync();
    if (customers.Count == 0) return Results.NotFound($"No customers found with name containing '{name}'");
    return Results.Ok(customers);
});

app.MapPost("/customers", async (CustomerCreatePayload payload, DarkKitchenDbContext db) =>
{
    var customer = new Customer
    {
        Name = payload.Name,
        Email = payload.Email
    };

    db.Customers.Add(customer);
    await db.SaveChangesAsync();
    Log.Information("Created customer {CustomerId} with name {CustomerName}", customer.Id, customer.Name);
    return Results.Created($"/customers/{customer.Id}", customer);
});


// Reports
app.MapGet("/reports/top-products", async (DarkKitchenDbContext db) =>
    await db.FulfillmentEvents
        .Where(f => f.Result == FulfillmentResult.Fulfilled)
        .Join(db.OrderLines, e => e.OrderId, l => l.OrderId, (e, l) => l)
        .GroupBy(l => l.DishId)
        .Select(g => new { DishId = g.Key, Units = g.Sum(l => l.Quantity) })
        .Join(db.Dishes, g => g.DishId, d => d.Id, (g, d) => new {g.Units, d.Name})
        .OrderByDescending(x => x.Units)
        .Take(3)
        .ToListAsync());

app.MapGet("/reports/top-customers", async (DarkKitchenDbContext db) =>
    Results.Ok(await db.FulfillmentEvents
        .Where(f => f.Result == FulfillmentResult.Fulfilled)
        .Join(db.Orders, e => e.OrderId, o => o.Id, (e, o) => o)
        .GroupBy(o => o.CustomerId)
        .Select(g => new { CustomerId = g.Key, Orders = g.Count() })
        .Join(db.Customers, g => g.CustomerId, c => c.Id, (g, c) => new { c.Name, g.Orders })
        .OrderByDescending(x => x.Orders)
        .Take(3)
        .ToListAsync()));

app.MapGet("/reports/fulfillment-rate", async (DarkKitchenDbContext db) =>
    Results.Ok(await db.FulfillmentEvents
        .GroupBy(g => g.Result)
        .Select(g => new { Result = g.Key.ToString(), Quantity = g.Count()})
        .ToListAsync()));


// Benchmarking and tests
app.MapPost("/benchmark", async () =>
{
    return "Test parallel vs sequential burst";
});

app.MapGet("/verify/no-oversell", async () =>
{
    return "Check for ingredients that have negative stock and report them";
});

app.Run();

Log.CloseAndFlush();

public record OrderLinePayload(int DishId, int Quantity);
public record OrderPayload(int CustomerId, List<OrderLinePayload> Lines);

public record CustomerCreatePayload(string Name, string Email);