using Microsoft.EntityFrameworkCore;
using Serilog;

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


app.MapGet("/", () => "Hello World!");

app.Run();

Log.CloseAndFlush();
