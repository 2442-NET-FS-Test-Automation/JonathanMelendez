using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Serilog;

using DarkKitchen.Api.Fulfillment;
using DarkKitchen.Data.Repository;
using DarkKitchen.Data.Exceptions;
using DarkKitchen.Data.Factories;
using DarkKitchen.Data.Entities;
using DarkKitchen.Data;

var builder = WebApplication.CreateBuilder(args);

// DB Stuff
var conn_string = "Server=localhost,1433;Database=DarkKitchenDB;User Id=sa;Password=mssql65.;TrustServerCertificate=true";
builder.Services.AddDbContextFactory<DarkKitchenDbContext>(options => options.UseSqlServer(conn_string));
builder.Services.AddDbContext<DarkKitchenDbContext>(options => options.UseSqlServer(conn_string),
    ServiceLifetime.Scoped, ServiceLifetime.Singleton);

// Services
builder.Services.AddScoped<IFulfillmentService, FulfillmentService>();
builder.Services.AddScoped<IDarkKitchenRepo, DarkKitchenRepoSqlServer>();
builder.Services.AddScoped<IReportsRepo, ReportsRepoSqlServer>();

builder.Services.AddScoped<CustomerFactory>();
builder.Services.AddScoped<OrderFactory>();


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
app.MapGet("/dish-menu", async (IDarkKitchenRepo repo, CancellationToken ct) =>
{
    var dishes = await repo.GetEnabledDishesAsync(ct);
    return Results.Ok(dishes.Select(d => new { d.Name, d.Description, d.OriginCountry, d.Price }));
});
       
app.MapGet("/dish-menu/by-price", async (IDarkKitchenRepo repo, CancellationToken ct) =>
{
    var dishes = await repo.GetEnabledDishesAsync(ct);
    return Results.Ok(
        dishes.Select(d => new { d.Name, d.Description, d.OriginCountry, d.Price })
        .OrderByDescending(d => d.Price)
    );
});

app.MapGet("/dish-menu/search/{id:int}", async (int id, IDarkKitchenRepo repo, CancellationToken ct) =>
{
    var dish = await repo.GetDishByIdAsync(id, ct);
    if (dish == null) return Results.NotFound($"No dish found with id '{id}'");
    return Results.Ok(new { dish.Name, dish.Description, dish.OriginCountry, dish.Price, Enabled = dish.Enabled.ToString() });
});

app.MapGet("/dish-menu/search/{name}", async (string name, IDarkKitchenRepo repo, CancellationToken ct) =>
{
    var dishes = await repo.GetDishesByNameAsync(name, ct);
    if (dishes.Count == 0) return Results.NotFound($"No dishes found with name containing '{name}'");
    return Results.Ok(
        dishes.Select(d => new { d.Id, d.Name, d.Description, d.OriginCountry, d.Price, Enabled = d.Enabled.ToString() })
    );
});

app.MapGet("/dish-menu/disabled", async (IDarkKitchenRepo repo, CancellationToken ct) =>
{
    var disabledDishes = await repo.GetDisabledDishesAsync(ct);
    if (disabledDishes.Count <= 0) return Results.NoContent();
    return Results.Ok(disabledDishes.Select(d => new { d.Id, d.Name, d.Description, d.OriginCountry, d.Price }));
});

app.MapPost("/dish-menu/toggle-enabled/{id:int}", async (int id, IDarkKitchenRepo repo, CancellationToken ct) =>
{
    try
    {
        var dish = await repo.ToggleDishEnabledAsync(id, ct);
        return Results.Ok(dish);
    }
    catch (DishNotFoundException e)
    {
        Log.Warning("Error while toggling Dish with id {id}", e.dishId);
        return Results.BadRequest("Dish not found");
    }
});


// Inventory stuff
app.MapGet("/inventory", async (IDarkKitchenRepo repo, CancellationToken ct) =>
{
    var inv = await repo.GetAllIngredientsAsync(ct);
    return Results.Ok(inv.Select(i => new { i.Name, Stock = $"{i.Stock} {i.Unit.GetAbbreviation()}" }));
});

app.MapGet("/inventory/search/{id:int}", async (int id, IDarkKitchenRepo repo, CancellationToken ct) =>
{
    var ing = await repo.GetIngredientByIdAsync(id, ct);
    if (ing == null) return Results.NotFound($"No ingredient found with id '{id}'");
    return Results.Ok(new { ing.Name, Stock = $"{ing.Stock} {ing.Unit.ToString()}" });  
});

app.MapGet("/inventory/search/{name}", async (string name, IDarkKitchenRepo repo, CancellationToken ct) =>
{
    var ingredients = await repo.GetIngredientsByNameAsync(name, ct);
    if (ingredients.Count == 0) return Results.NotFound($"No ingredients found with name containing '{name}'");
    return Results.Ok(ingredients.Select(i => new { i.Name, Stock = $"{i.Stock} {i.Unit.ToString()}" }));
});

app.MapGet("/inventory/by-stock", async (IDarkKitchenRepo repo, CancellationToken ct) =>
{
    var ings = await repo.GetAllIngredientsAsync(ct);
    return Results.Ok(
        ings.Select(i => new { i.Name, i.Stock, Unit = i.Unit.ToString() })
        .OrderByDescending(i => i.Stock)
    ); 
});

app.MapGet("/inventory/below-stock/{qty:int}", async (int qty, IDarkKitchenRepo repo, CancellationToken ct) =>
{
    var ings = await repo.GetIngredientsBelowStockAsync(qty, ct);
    if(ings.Count == 0) return Results.NoContent();
    return Results.Ok(ings.OrderBy(i => i.Stock));
});

app.MapPost("/inventory/reset", async (IDarkKitchenRepo repo, CancellationToken ct) =>
{
    await repo.IngredientsResetStock(ct);
    return Results.Ok("Stock reset");
});


// Orders
app.MapGet("/orders", async (IDarkKitchenRepo repo, CancellationToken ct) => 
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

app.MapGet("/orders/last/{qty:int}", async (int qty, IDarkKitchenRepo repo, CancellationToken ct) => 
{
    var orders = await repo.GetAllOrdersAsync(ct);
    return Results.Ok(
        orders.Select(o => new {
            o.Id,
            o.CustomerId,
            Status = o.Status.ToString(),
            Lines = o.Lines.Select(l => new { l.DishId, l.Quantity })
        })
        .OrderByDescending(o => o.Id)
        .Take(qty)
    );
});

app.MapGet("/orders/{orderId:int}", async (int orderId, IDarkKitchenRepo repo, CancellationToken ct) =>
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

app.MapGet("/orders/customer/{customerId:int}", async (int customerId, IDarkKitchenRepo repo, CancellationToken ct) =>
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
    IDarkKitchenRepo repo,
    OrderFactory orderFactory,
    IFulfillmentService fSvc,
    CancellationToken ct
) => {
    // We check that the customer exists
    if (await repo.GetCustomerByIdAsync(payload.CustomerId, ct) == null)
        return Results.BadRequest($"Customer {payload.CustomerId} not found");

    // We check that every Dish requested actually exists
    foreach (var line in payload.Lines)
    {
        var dish = await repo.GetDishByIdAsync(line.DishId, ct);
        if (dish is null) 
            return Results.NotFound($"Dish {line.DishId} not found");
        else if (dish.Enabled == false)
            return Results.BadRequest($"Dish {line.DishId} is disabled");
    }

    Order newOrder = orderFactory.CreateOrder("normal", payload.CustomerId, payload.Lines.Select(l => (l.DishId, l.Quantity)));

    await repo.AddOrderAsync(newOrder, ct);
    Log.Information("Created order {OrderId} for customer {CustomerId}", newOrder.Id, newOrder.CustomerId);

    FulfillmentResult result = await fSvc.FulfillOneAsync(newOrder.Id, ct);

    return Results.Created($"/orders/{newOrder.Id}", new { OrderId = newOrder.Id, FulfillmentResult = result.ToString() });
});

app.MapPost("/orders/burst", async(
    BurstOrderPayload payload, 
    IDarkKitchenRepo repo,
    OrderFactory orderFactory,
    IServiceScopeFactory scopes,
    IHostApplicationLifetime lifetime,
    CancellationToken ct
) => {
    // Verify all customers are valid
    var customerIds = payload.Orders.Select(o => o.CustomerId).ToList();
    var existingCustomers = await repo.GetCustomersByIdsAsync(customerIds, ct);

    var missingCustomers = customerIds.Except(existingCustomers.Select(c => c.Id)).ToList();
    if (missingCustomers.Count > 0)
        return Results.BadRequest($"Customers not found: {string.Join(", ", missingCustomers)}");

    // Verify all dishes are valid
    var dishIds = payload.Orders
        .SelectMany(o => o.Lines.Select(l => l.DishId))
        .Distinct()
        .ToList();

    var enabledDishes = await repo.GetEnabledDishesAsync(ct);
    var existingDishes = enabledDishes.Where(d => dishIds.Contains(d.Id));

    var missingDishes = dishIds.Except(existingDishes.Select(d => d.Id)).ToList();
    if (missingDishes.Count > 0)
        return Results.BadRequest($"Dishes not found or disabled: {string.Join(", ", missingDishes)}");

    // Create orders
    var orders = new List<Order>();
    foreach (var o in payload.Orders)
    {
        orders.Add(orderFactory.CreateOrder("normal", o.CustomerId, o.Lines.Select(l => (l.DishId, l.Quantity))));
    }

    // Save to generate IDs and persist the orders
    await repo.AddOrdersAsync(orders, ct);

    var orderIds = orders.Select(o => o.Id).ToList();
    Log.Information("Created {OrderCount} orders: {OrderIds}", orderIds.Count, string.Join(", ", orderIds));

    var burstTask = Task.Run( async () =>
    {
        try
        {
            using var scope = scopes.CreateScope(); // ask for a fresh scope
            var service  = scope.ServiceProvider.GetRequiredService<IFulfillmentService>(); //grab a fulfillment service
            await service.FulfillBurstAsync(orderIds, 2, ct); // use it to call fulfillBurstAsync()
        }
        catch (Exception e)
        {
            // Fail in silence
            Log.Error(e, "Burst fulfillment failed");
        }
    }, ct);

    lifetime.ApplicationStopping.Register(() =>
    {
        Log.Information("Shutting down, waiting for burst to finish...");
        var completed = burstTask.Wait(TimeSpan.FromSeconds(30));
        if (!completed)
            Log.Warning("Burst did not complete within timeout, continuing shutdown.");
    });

    return Results.Accepted();
});


// Customers
app.MapGet("/customers", async (IDarkKitchenRepo repo, CancellationToken ct) => 
{
    var customers = await repo.GetAllCustomersAsync(ct);
    return Results.Ok(customers.Select(c => new { c.Id, c.Name, c.Email }));
});

app.MapGet("/customers/{customerId:int}", async (int customerId, IDarkKitchenRepo repo, CancellationToken ct) => 
{
    var customer = await repo.GetCustomerByIdAsync(customerId, ct);

    if (customer is null) return Results.NotFound($"Customer {customerId} not found");
    return Results.Ok(new { customer.Id, customer.Name, customer.Email });
});

app.MapGet("/customers/search/{name}", async (string name, IDarkKitchenRepo repo, CancellationToken ct) => 
{
    var customers = await repo.GetCustomerByNameAsync(name, ct);
    if (customers.Count == 0) return Results.NotFound($"No customers found with name containing '{name}'");
    return Results.Ok(customers.Select(c => new { c.Id, c.Name, c.Email }));
});

app.MapPost("/customers", async (
    CustomerCreatePayload payload, 
    IDarkKitchenRepo repo,
    CustomerFactory customerFactory,
    CancellationToken ct
) => {
    var customer = await repo.AddCustomerAsync(customerFactory.CreateCustomer(payload.Name, payload.Email), ct);

    Log.Information("Created customer {CustomerId} with name {CustomerName}", customer.Id, customer.Name);
    return Results.Created($"/customers/{customer.Id}", customer);
});

app.MapDelete("/customers", async (int customerId, IDarkKitchenRepo repo, CancellationToken ct) =>
{
    try
    {
        await repo.DeleteCustomerByIdAsync(customerId, ct);
        return Results.Ok($"Customer with id {customerId} was deleted");
    }
    catch
    {
        return Results.NotFound("Customer with id {customer.Id} not found");
    }
});

// Reports
app.MapGet("/reports/dishes/ranking", async (IReportsRepo repo, CancellationToken ct) => Results.Ok(await repo.GetDishesRankedReport(ct)));

app.MapGet("/reports/dishes/ranking/{rank:int}", async (int rank, IReportsRepo repo, CancellationToken ct) =>
{
    if (rank == 0) return Results.NotFound($"Rank {rank} not in range");
    var rankedDishes = await repo.GetDishesRankedReport(ct);

    var dish = rankedDishes
        .Skip(rank - 1)
        .Take(1)
        .FirstOrDefault();

    if (dish is null) return Results.NotFound($"Rank {rank} not in range");
    return Results.Ok(dish);
});

app.MapGet("reports/dishes/ranking/{dishName}", async (string dishName, IReportsRepo repo, CancellationToken ct) =>
{
    var dishesByRank = await repo.GetDishesRankedReport(ct);

    var rankedDishes = 
        dishesByRank.Select((x, index) => new {
                Rank = index + 1,
                x.Name, 
                x.Units
            })
            .OrderBy(o => o.Name)
            .ToList();

    var target = new { Rank = 0, Name = dishName, Units = 0 };
    
    int res = rankedDishes.BinarySearch(target, Comparer<dynamic>.Create((a, b) =>
        {
            string nameA = (a).Name;
            string nameB = (b).Name;
            return string.Compare(nameA, nameB, StringComparison.OrdinalIgnoreCase);
        }));
    
    if (res < 0) return Results.NotFound($"Dish {dishName} not found");
    return Results.Ok(rankedDishes[res].Rank);
});

app.MapGet("/reports/dishes/ranking/top-{qty:int}", async (int qty, IReportsRepo repo, CancellationToken ct) =>
{
    var rankedDishes = await repo.GetDishesRankedReport(ct);
    return rankedDishes.Take(qty);
});

app.MapGet("/reports/top-customers/top-{qty:int}", async (int qty, IReportsRepo repo, CancellationToken ct) =>
{
    var rankedCustomers = await repo.GetCustomersRankedReport(ct);
    return rankedCustomers.Take(qty);
});

app.MapGet("/reports/fulfillment-rate", async (IReportsRepo repo, CancellationToken ct) => Results.Ok(await repo.GetFulfillmentRateReport(ct)));


// Benchmarking and tests
app.MapPost("/benchmark/{MaxConcurrentFulfillments:int=2}", async (
    int MaxConcurrentFulfillments,
    BurstOrderPayload payload, 
    IDarkKitchenRepo repo,
    OrderFactory orderFactory,
    IFulfillmentService fs,
    CancellationToken ct
) => {
    // Verify all customers are valid
    var customerIds = payload.Orders.Select(o => o.CustomerId).ToList();
    var existingCustomers = await repo.GetCustomersByIdsAsync(customerIds, ct);

    var missingCustomers = customerIds.Except(existingCustomers.Select(c => c.Id)).ToList();
    if (missingCustomers.Count > 0)
        return Results.BadRequest($"Customers not found: {string.Join(", ", missingCustomers)}");

    // Verify all dishes are valid
    var dishIds = payload.Orders
        .SelectMany(o => o.Lines.Select(l => l.DishId))
        .Distinct()
        .ToList();

    var existingDishes = await repo.GetDishesByIdsAsync(dishIds, ct);

    var missingDishes = dishIds.Except(existingDishes.Select(d => d.Id)).ToList();
    if (missingDishes.Count > 0)
        return Results.BadRequest($"Dishes not found: {string.Join(", ", missingDishes)}");

    // Reset stocks
    await repo.IngredientsResetStock(ct);

    // Create first batch for sequential
    var ordersS = new List<Order>();
    foreach (var o in payload.Orders)
    {
        ordersS.Add(orderFactory.CreateOrder("normal", o.CustomerId, o.Lines.Select(l => (l.DishId, l.Quantity))));
    }

    // Save to generate IDs and persist the orders
    await repo.AddOrdersAsync(ordersS, ct);

    // Sequential execution
    var sw1 = Stopwatch.StartNew();
    foreach (var order in ordersS)
        await fs.FulfillOneAsync(order.Id, ct);
    sw1.Stop();

    // Reset stocks
    await repo.IngredientsResetStock(ct);

    // Second batch for Parallel
    var ordersP = new List<Order>();
    foreach (var o in payload.Orders)
    {
        ordersP.Add(orderFactory.CreateOrder("normal", o.CustomerId, o.Lines.Select(l => (l.DishId, l.Quantity))));
    }

    // Save to generate IDs and persist the orders
    await repo.AddOrdersAsync(ordersP, ct);

    // Parallel exec
    var sw2 = Stopwatch.StartNew();
    await fs.FulfillBurstAsync(
        ordersP.Select(o => o.Id).ToList(), 
        MaxConcurrentFulfillments, 
        ct);
    sw2.Stop();

    var sequentialMs = sw1.ElapsedMilliseconds;
    var concurrentMs = sw2.ElapsedMilliseconds;

    return Results.Ok(new {
        sequentialMs,
        concurrentMs,
        speedup = $"{(double)sequentialMs/concurrentMs:F2}"
    });
});

app.MapGet("/verify/no-oversell", async (IDarkKitchenRepo repo, CancellationToken ct) =>
{
    var oversold = await repo.GetIngredientsBelowStockAsync(0, ct);
    if (oversold.Count > 0) return Results.Ok(oversold);
    return Results.NoContent();
});

app.Run();

Log.CloseAndFlush();

public record OrderLinePayload(int DishId, int Quantity);
public record OrderPayload(int CustomerId, List<OrderLinePayload> Lines);

public record BurstOrderPayload(List<OrderPayload> Orders );

public record CustomerCreatePayload(string Name, string Email);