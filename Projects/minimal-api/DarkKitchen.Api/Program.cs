using Microsoft.EntityFrameworkCore;
using Serilog;

using DarkKitchen.Data.Entities;
using DarkKitchen.Data;

var builder = WebApplication.CreateBuilder(args);

var conn_string = "Server=localhost,1433;Database=mssql_test;User Id=sa;Password=mssql65.;TrustServerCertificate=true";
builder.Services.AddDbContext<DarkKitchenDbContext>(options => options.UseSqlServer(conn_string),
    ServiceLifetime.Scoped, ServiceLifetime.Singleton);

builder.Services.AddDbContextFactory<DarkKitchenDbContext>(options => options.UseSqlServer(conn_string));

//builder.Services.AddScoped<IFulfillmentService, FulfillmentService>();

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


app.MapGet("/seed", () =>
{
    return "Seed items here";
});

app.MapGet("/inventory", () =>
{
    return "See the stock here";
});

app.MapGet("/reset", () =>
{
    return "Reset stock here";
});

app.MapGet("/order", () =>
{
    return "You place one order here";
});

app.MapGet("/order-burst", () =>
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
    return "Ordered list of best selling products";
});

app.MapGet("/reports/worst-customers", () =>
{
    return "Ordered list of worst selling products";
});

app.MapGet("/reports/fulfillment-rate", () =>
{
    return "Ordered list of worst selling products";
});


// Benchmarking
app.MapGet("/benchmark", () =>
{
    return "Test parallel vs sequential burst";
});

app.Run();

Log.CloseAndFlush();
