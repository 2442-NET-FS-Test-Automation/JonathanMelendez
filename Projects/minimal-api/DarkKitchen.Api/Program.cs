using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Serilog;

using DarkKitchen.Api.Fulfillment;
using DarkKitchen.Data.Repository;
using DarkKitchen.Data.Entities;
using DarkKitchen.Data.Defaults;
using DarkKitchen.Data;

var builder = WebApplication.CreateBuilder(args);

// DB Stuff
var conn_string = "Server=localhost,1433;Database=DarkKitchenDB;User Id=sa;Password=mssql65.;TrustServerCertificate=true";
builder.Services.AddDbContext<DarkKitchenDbContext>(options => options.UseSqlServer(conn_string),
    ServiceLifetime.Scoped, ServiceLifetime.Singleton);
builder.Services.AddDbContextFactory<DarkKitchenDbContext>(options => options.UseSqlServer(conn_string));

// Services
builder.Services.AddScoped<IFulfillmentService, FulfillmentService>();
builder.Services.AddScoped<IOrderRepo, OrderRepoSqlServer>();
builder.Services.AddScoped<IInventoryRepo, InventoryRepoSqlServer>();

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
app.MapGet("/dish-menu", async (DarkKitchenDbContext db) => 
    Results.Ok(await db.Dishes
        .Where(d => d.Enabled == true)
        .Select(d => new { d.Name, d.Description, d.OriginCountry, d.Price })
        .ToListAsync()));

app.MapGet("/dish-menu/by-price", async (DarkKitchenDbContext db) => 
    Results.Ok(await db.Dishes
        .Select(d => new { d.Name, d.Description, d.OriginCountry, d.Price })
        .OrderByDescending(d => d.Price)
        .ToListAsync()));

app.MapGet("/dish-menu/search/{id:int}", async (int id, DarkKitchenDbContext db) =>
{
    var dish = await db.Dishes
        .Where(d => d.Id == id)
        .Select(d => new { d.Name, d.Description, d.OriginCountry, d.Price, Enabled = d.Enabled.ToString() })
        .FirstOrDefaultAsync();
    if (dish == null) return Results.NotFound($"No dish found with id '{id}'");
    return Results.Ok(dish);
});

app.MapGet("/dish-menu/search/{name}", async (string name, DarkKitchenDbContext db) =>
{
    var dishes = await db.Dishes
        .Select(d => new { d.Id, d.Name, d.Description, d.OriginCountry, d.Price, Enabled = d.Enabled.ToString() })
        .Where(d => d.Name.ToLower().Contains(name.ToLower()))
        .ToListAsync();
    if (dishes.Count() == 0) return Results.NotFound($"No dishes found with name containing '{name}'");
    return Results.Ok(dishes);
});

app.MapGet("/dish-menu/disabled", async (DarkKitchenDbContext db) =>
{
    var disabledDishes = await db.Dishes
        .Where(d => d.Enabled == false)
        .Select(d => new { d.Id, d.Name, d.Description, d.OriginCountry, d.Price })
        .ToListAsync();
    if (disabledDishes.Count > 0) return Results.Ok(disabledDishes);
    return Results.NoContent();
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

app.MapGet("/inventory/low-stock", async (DarkKitchenDbContext db) => 
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
app.MapGet("/orders", async (IOrderRepo repo, CancellationToken ct) => 
{
    var orders = await repo.GetAllOrdersAsync(ct);
    return Results.Ok(
        orders.Select(o => new {
            o.Id,
            o.CustomerId,
            Status = o.Status.ToString(),
            Lines = o.Lines.Select(l => new { l.DishId, l.Quantity })
        }
    ));
});

app.MapGet("/orders/{orderId:int}", async (int orderId, IOrderRepo repo, CancellationToken ct) =>
{
    var order = await repo.GetOrderByIdAsync(orderId, ct);
    if (order == null) return Results.NotFound($"Order {orderId} not found");
    return Results.Ok(new { 
        order.Id,
        order.CustomerId, 
        Status = order.Status.ToString(), 
        Lines = order.Lines.Select(l => new { l.DishId, l.Quantity }) 
    });
});

app.MapGet("/orders/customer/{customerId:int}", async (int customerId, IOrderRepo repo, CancellationToken ct) =>
{
    var orders = await repo.GetOrdersForCustomerAsync(customerId, ct);
    if (orders.Count == 0) return Results.NotFound($"No orders found for customer {customerId}");
    return Results.Ok(
        orders.Select(o => new { 
            o.Id, 
            o.CustomerId, 
            Status = o.Status.ToString(), 
            Lines = o.Lines.Select(l => new { l.DishId, l.Quantity }) 
        }));
});

app.MapPost("/orders/single", async (
    OrderPayload payload, 
    IOrderRepo repo, 
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

    await repo.AddOrderAsync(newOrder, ct);
    Log.Information("Created order {OrderId} for customer {CustomerId}", newOrder.Id, newOrder.CustomerId);

    FulfillmentResult result = await fSvc.FulfillOneAsync(newOrder.Id, ct);

    return Results.Created($"/orders/{newOrder.Id}", new { OrderId = newOrder.Id, FulfillmentResult = result.ToString() });
});

app.MapPost("/orders/burst", async(
    BurstOrderPayload payload, 
    IOrderRepo repo,
    DarkKitchenDbContext db,
    IServiceScopeFactory scopes,
    IHostApplicationLifetime lifetime
) => {
    var ct = lifetime.ApplicationStopping;

    // Verify all customers are valid
    var customerIds = payload.Orders.Select(o => o.CustomerId).ToList();
    var existingCustomers = await db.Customers
        .Where(c => customerIds.Contains(c.Id))
        .Select(c => c.Id)
        .ToListAsync(ct);

    var missingCustomers = customerIds.Except(existingCustomers).ToList();
    if (missingCustomers.Count > 0)
        return Results.BadRequest($"Customers not found: {string.Join(", ", missingCustomers)}");

    // verify all dishes are valid
    var dishIds = payload.Orders
        .SelectMany(o => o.Lines.Select(l => l.DishId))
        .Distinct()
        .ToList();

    var existingDishes = await db.Dishes
        .Where(d => dishIds.Contains(d.Id))
        .Select(d => d.Id)
        .ToListAsync(ct);

    var missingDishes = dishIds.Except(existingDishes).ToList();
    if (missingDishes.Count > 0)
        return Results.BadRequest($"Dishes not found: {string.Join(", ", missingDishes)}");
    
    // Create orders
    var orders = payload.Orders.Select(o => new Order
        {
            CustomerId = o.CustomerId,
            Status = OrderStatus.Pending,
            Lines = o.Lines.Select(l => new OrderLine
            {
                DishId = l.DishId,
                Quantity = l.Quantity
            }).ToList()
        }).ToList();

    // Save to generate IDs and persist the orders
    await repo.AddOrdersAsync(orders, ct);

    var orderIds = orders.Select(o => o.Id).ToList();
    Log.Information("Created {OrderCount} orders: {OrderIds}", orderIds.Count, string.Join(", ", orderIds));

    _ = Task.Run( async () => // assigning the task result to a discard runs this as a background task
    {
        try
        {
            using var scope = scopes.CreateScope(); // ask for a fresh scope
            var service  = scope.ServiceProvider.GetRequiredService<IFulfillmentService>(); //grab a fulfillment service
            await service.FulfillBurstAsync(orderIds, ct); // use it to call fulfillBurstAsync()
        }
        catch (Exception e)
        {
            // Fail in silence
            Log.Error(e, "Burst fulfillment failed");
        }
    }, ct);

    return Results.Accepted();
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

app.MapDelete("/customers", async (int customerId, DarkKitchenDbContext db) =>
{
    var customer = await db.Customers.Where(c => c.Id == customerId).FirstAsync();
    
    if (customer is null) return Results.NotFound("Customer with id {customer.Id} not found");
    
    db.Customers.Remove(customer);
    db.SaveChanges();

    return Results.Ok($"Customer {customer.Name} with id {customer.Id} was deleted");
});

// Reports
app.MapGet("/reports/dishes/ranking", async (DarkKitchenDbContext db) =>
    Results.Ok(await db.FulfillmentEvents
        .Where(f => f.Result == FulfillmentResult.Fulfilled)
        .Join(db.OrderLines, e => e.OrderId, l => l.OrderId, (e, l) => l)
        .GroupBy(l => l.DishId)
        .Select(g => new { DishId = g.Key, Units = g.Sum(l => l.Quantity) })
        .Join(db.Dishes, g => g.DishId, d => d.Id, (g, d) => new {d.Name, UnitsSold = g.Units})
        .OrderByDescending(x => x.UnitsSold)
        .ToListAsync()));

app.MapGet("/reports/dishes/ranking/{rank:int}", async (int rank, DarkKitchenDbContext db) =>
{
    var dish = await db.FulfillmentEvents
        .Where(f => f.Result == FulfillmentResult.Fulfilled)
        .Join(db.OrderLines, e => e.OrderId, l => l.OrderId, (e, l) => l)
        .GroupBy(l => l.DishId)
        .Select(g => new { DishId = g.Key, Units = g.Sum(l => l.Quantity) })
        .Join(db.Dishes, g => g.DishId, d => d.Id, (g, d) => new {d.Name, UnitsSold = g.Units})
        .OrderByDescending(x => x.UnitsSold)
        .Skip(rank-1)
        .Take(1)
        .FirstAsync();
    
    if (dish is null) return Results.NotFound($"Rank {rank} not in range");
    return Results.Ok(dish);
});

app.MapGet("reports/dishes/ranking/{dishName}", async (string dishName, DarkKitchenDbContext db) =>
{
    var dishesByRank = await db.FulfillmentEvents
        .Where(f => f.Result == FulfillmentResult.Fulfilled)
        .Join(db.OrderLines, e => e.OrderId, l => l.OrderId, (e, l) => l)
        .GroupBy(l => l.DishId)
        .Select(g => new { DishId = g.Key, Units = g.Sum(l => l.Quantity) })
        .Join(db.Dishes, g => g.DishId, d => d.Id, (g, d) => new {d.Name, UnitsSold = g.Units})
        .OrderByDescending(x => x.UnitsSold)
        .ToListAsync();

    var rankedDishes = 
        dishesByRank.Select((x, index) => new 
            {
                Rank = index + 1,
                x.Name, 
                x.UnitsSold 
            })
            .OrderBy(o => o.Name)
            .ToList();

    var target = new { Rank = 0, Name = dishName, UnitsSold = 0 };
    
    int res = rankedDishes.BinarySearch(target, Comparer<dynamic>.Create((a, b) =>
        {
            string nameA = (a).Name;
            string nameB = (b).Name;
            return string.Compare(nameA, nameB, StringComparison.OrdinalIgnoreCase);
        }));
    
    if (res < 0) return Results.NotFound($"");
    return Results.Ok(rankedDishes[res].Rank);
});

app.MapGet("/reports/dishes/ranking/top-{qty:int}", async (int qty, DarkKitchenDbContext db) =>
    await db.FulfillmentEvents
        .Where(f => f.Result == FulfillmentResult.Fulfilled)
        .Join(db.OrderLines, e => e.OrderId, l => l.OrderId, (e, l) => l)
        .GroupBy(l => l.DishId)
        .Select(g => new { DishId = g.Key, Units = g.Sum(l => l.Quantity) })
        .Join(db.Dishes, g => g.DishId, d => d.Id, (g, d) => new {d.Name, UnitsSold = g.Units})
        .OrderByDescending(x => x.UnitsSold)
        .Take(qty)
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
app.MapPost("/benchmark", async (
    BurstOrderPayload payload, 
    IOrderRepo repo,
    IFulfillmentService fs,
    DarkKitchenDbContext db,
    CancellationToken ct
) => {
    // Verify all customers are valid
    var customerIds = payload.Orders.Select(o => o.CustomerId).ToList();
    var existingCustomers = await db.Customers
        .Where(c => customerIds.Contains(c.Id))
        .Select(c => c.Id)
        .ToListAsync(ct);

    var missingCustomers = customerIds.Except(existingCustomers).ToList();
    if (missingCustomers.Count > 0)
        return Results.BadRequest($"Customers not found: {string.Join(", ", missingCustomers)}");

    // verify all dishes are valid
    var dishIds = payload.Orders
        .SelectMany(o => o.Lines.Select(l => l.DishId))
        .Distinct()
        .ToList();

    var existingDishes = await db.Dishes
        .Where(d => dishIds.Contains(d.Id))
        .Select(d => d.Id)
        .ToListAsync(ct);

    var missingDishes = dishIds.Except(existingDishes).ToList();
    if (missingDishes.Count > 0)
        return Results.BadRequest($"Dishes not found: {string.Join(", ", missingDishes)}");
    
    // Reset Stocks
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

    // Create first batch for sequential
    var ordersS = payload.Orders.Select(o => new Order
        {
            CustomerId = o.CustomerId,
            Status = OrderStatus.Pending,
            Lines = o.Lines.Select(l => new OrderLine
            {
                DishId = l.DishId,
                Quantity = l.Quantity
            }).ToList()
        }).ToList();

    // Save to generate IDs and persist the orders
    await repo.AddOrdersAsync(ordersS, ct);

    // Sequential execution
    var sw1 = Stopwatch.StartNew();
    foreach (var order in ordersS)
        await fs.FulfillOneAsync(order.Id, ct);
    sw1.Stop();

    // Reset stocks
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

    // Second batch for Parallel
    var ordersP = payload.Orders.Select(o => new Order
        {
            CustomerId = o.CustomerId,
            Status = OrderStatus.Pending,
            Lines = o.Lines.Select(l => new OrderLine
            {
                DishId = l.DishId,
                Quantity = l.Quantity
            }).ToList()
        }).ToList();

    // Save to generate IDs and persist the orders
    await repo.AddOrdersAsync(ordersP, ct);

    // Parallel exec
    var sw2 = Stopwatch.StartNew();
    await fs.FulfillBurstAsync(ordersP.Select(o => o.Id).ToList(), ct);
    sw2.Stop();

    var sequentialMs = sw1.ElapsedMilliseconds;
    var concurrentMs = sw2.ElapsedMilliseconds;

    return Results.Ok(new
    {
        sequentialMs,
        concurrentMs,
        speedup = $"{(double)sequentialMs/concurrentMs:F2}"
    });
});

app.MapGet("/verify/no-oversell", async (DarkKitchenDbContext db, CancellationToken ct) =>
{
    var oversold = await db.Ingredients.Where(i => i.Stock < 0).ToListAsync(ct);
    if (oversold.Count > 0) return Results.Ok(oversold);
    return Results.NoContent();
});

app.Run();

Log.CloseAndFlush();

public record OrderLinePayload(int DishId, int Quantity);
public record OrderPayload(int CustomerId, List<OrderLinePayload> Lines);

public record BurstOrderPayload(List<OrderPayload> Orders );

public record CustomerCreatePayload(string Name, string Email);